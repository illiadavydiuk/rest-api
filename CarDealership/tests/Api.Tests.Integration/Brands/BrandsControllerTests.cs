using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.Dtos;
using Domain.Brands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Brands;
using Xunit;

namespace Api.Tests.Integration.Brands;

public class BrandsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Brand _createValidTestBrand;
    private readonly Brand _createTakenTestBrand;
    private readonly Brand _createInvalidTestBrand;
    private readonly Brand _updateValidTestBrand;
    private readonly Brand _updateTakenTestBrand;
    private readonly Brand _updateInvalidTestBrand;

    private readonly string _createBrandRoute;
    private readonly string _getExistingBrandRoute;
    private readonly string _getNonExistingBrandRoute;
    private readonly string _updateExistingBrandRoute;
    private readonly string _updateNonExistingBrandRoute;
    private readonly string _deleteExistingBrandRoute;
    private readonly string _deleteNonExistingBrandRoute;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BrandsControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
        _createValidTestBrand = BrandsTestData.CreateValidTestBrand();
        _createTakenTestBrand = BrandsTestData.CreateTakenTestBrand();
        _createInvalidTestBrand = BrandsTestData.CreateInvalidTestBrand();
        _updateValidTestBrand = BrandsTestData.UpdateValidTestBrand();
        _updateTakenTestBrand = BrandsTestData.UpdateTakenTestBrand();
        _updateInvalidTestBrand = BrandsTestData.UpdateInvalidTestBrand();

        var nonExistentBrandId = BrandsTestData.NonExistentTestBrandId();

        _createBrandRoute = "brands";
        _getExistingBrandRoute = $"brands/{_createTakenTestBrand.Id.Value}";
        _getNonExistingBrandRoute = $"brands/{nonExistentBrandId}";
        _updateExistingBrandRoute = $"brands/{_updateTakenTestBrand.Id.Value}";
        _updateNonExistingBrandRoute = $"brands/{nonExistentBrandId}";
        _deleteExistingBrandRoute = $"brands/{_createTakenTestBrand.Id.Value}";
        _deleteNonExistingBrandRoute = $"brands/{nonExistentBrandId}";
    }

    public async Task InitializeAsync()
    {
        await Context.Brands.AddAsync(_createTakenTestBrand);
        await Context.Brands.AddAsync(_updateTakenTestBrand);

        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        var testIds = new[]
        {
            _createTakenTestBrand.Id,
            _updateTakenTestBrand.Id
        };

        var brandsToRemove = await Context.Brands
            .Where(b => testIds.Contains(b.Id) || b.Name == _createValidTestBrand.Name || b.Name == _updateValidTestBrand.Name)
            .ToListAsync();

        Context.Brands.RemoveRange(brandsToRemove);

        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ShouldGetAllBrands()
    {
        // Act
        var response = await Client.GetAsync(_createBrandRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var brands = await response.Content.ReadFromJsonAsync<List<BrandDto>>(JsonOptions);
        brands.Should().NotBeNull();
        brands!.Should().Contain(b => b.Id == _createTakenTestBrand.Id.Value);
        brands.Should().Contain(b => b.Id == _updateTakenTestBrand.Id.Value);
    }

    [Fact]
    public async Task ShouldGetBrandById()
    {
        // Act
        var response = await Client.GetAsync(_getExistingBrandRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var brand = await response.Content.ReadFromJsonAsync<BrandDto>(JsonOptions);
        brand.Should().NotBeNull();
        brand!.Id.Should().Be(_createTakenTestBrand.Id.Value);
        brand.Name.Should().Be(_createTakenTestBrand.Name);
        brand.Country.Should().Be(_createTakenTestBrand.Country);
    }

    [Fact]
    public async Task ShouldNotGetBrandBecauseNotFound()
    {
        // Act
        var response = await Client.GetAsync(_getNonExistingBrandRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldCreateBrand()
    {
        // Arrange
        var request = new CreateBrandDto(
            _createValidTestBrand.Name,
            _createValidTestBrand.Country);

        // Act
        var response = await Client.PostAsJsonAsync(_createBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseDto = await response.Content.ReadFromJsonAsync<BrandDto>(JsonOptions);
        responseDto.Should().NotBeNull();

        var dbBrand = await Context.Brands.FirstOrDefaultAsync(x => x.Id == new BrandId(responseDto!.Id));
        dbBrand.Should().NotBeNull();
        dbBrand!.Name.Should().Be(_createValidTestBrand.Name);
        dbBrand.Country.Should().Be(_createValidTestBrand.Country);
    }

    [Fact]
    public async Task ShouldNotCreateBrandBecauseNameIsAlreadyTaken()
    {
        // Arrange
        var request = new CreateBrandDto(
            _createTakenTestBrand.Name,
            _createTakenTestBrand.Country);

        // Act
        var response = await Client.PostAsJsonAsync(_createBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateBrandBecauseRequestIsInvalid()
    {
        // Arrange
        var brandsInDb = await Context.Brands.CountAsync();
        var request = new CreateBrandDto(
            _createInvalidTestBrand.Name,
            _createInvalidTestBrand.Country);

        // Act
        var response = await Client.PostAsJsonAsync(_createBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbBrandCount = await Context.Brands.CountAsync();
        dbBrandCount.Should().Be(brandsInDb);
    }

    [Fact]
    public async Task ShouldUpdateBrand()
    {
        // Arrange
        var request = new UpdateBrandDto(
            _updateValidTestBrand.Name,
            _updateValidTestBrand.Country);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseDto = await response.Content.ReadFromJsonAsync<BrandDto>(JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_updateTakenTestBrand.Id.Value);

        var dbBrand = await Context.Brands.FirstOrDefaultAsync(x => x.Id == _updateTakenTestBrand.Id);
        dbBrand.Should().NotBeNull();
        dbBrand!.Name.Should().Be(_updateValidTestBrand.Name);
        dbBrand.Country.Should().Be(_updateValidTestBrand.Country);
    }

    [Fact]
    public async Task ShouldNotUpdateBrandBecauseNameIsAlreadyTaken()
    {
        // Arrange
        var request = new UpdateBrandDto(
            _createTakenTestBrand.Name,
            _updateValidTestBrand.Country);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbBrand = await Context.Brands.SingleAsync(x => x.Id == _updateTakenTestBrand.Id);
        dbBrand.Name.Should().Be(_updateTakenTestBrand.Name);
        dbBrand.Country.Should().Be(_updateTakenTestBrand.Country);
    }

    [Fact]
    public async Task ShouldNotUpdateBrandBecauseRequestIsInvalid()
    {
        // Arrange
        var request = new UpdateBrandDto(
            _updateInvalidTestBrand.Name,
            _updateInvalidTestBrand.Country);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbBrand = await Context.Brands.SingleAsync(x => x.Id == _updateTakenTestBrand.Id);
        dbBrand.Name.Should().Be(_updateTakenTestBrand.Name);
    }

    [Fact]
    public async Task ShouldNotUpdateBrandBecauseBrandNotFound()
    {
        // Arrange
        var request = new UpdateBrandDto(
            _updateValidTestBrand.Name,
            _updateValidTestBrand.Country);

        // Act
        var response = await Client.PutAsJsonAsync(_updateNonExistingBrandRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteBrand()
    {
        // Act
        var response = await Client.DeleteAsync(_deleteExistingBrandRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbBrand = await Context.Brands.FirstOrDefaultAsync(x => x.Id == _createTakenTestBrand.Id);
        dbBrand.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteBrandBecauseBrandNotFound()
    {
        // Arrange
        var dbBrandsCount = await Context.Brands.CountAsync();

        // Act
        var response = await Client.DeleteAsync(_deleteNonExistingBrandRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var dbBrandCount = await Context.Brands.CountAsync();
        dbBrandCount.Should().Be(dbBrandsCount);
    }
}