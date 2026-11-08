using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Dtos;
using Domain.Cars;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Cars;
using Xunit;

namespace Api.Tests.Integration.Cars;

public class CarsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Car _createValidTestCar;
    private readonly Car _createTakenTestCar;
    private readonly Car _createInvalidTestCar;
    private readonly Car _updateValidTestCar;
    private readonly Car _updateTakenTestCar;
    private readonly Car _updateInvalidTestCar;

    private readonly string _createCarRoute;
    private readonly string _updateExistingCarRoute;
    private readonly string _updateNonExistingCarRoute;
    private readonly string _deleteExistingCarRoute;
    private readonly string _deleteNonExistingCarRoute;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public CarsControllerTests(IntegrationTestWebFactory factory)
        : base(factory)
    {
        _createValidTestCar = CarsTestData.CreateValidTestCar();
        _createTakenTestCar = CarsTestData.CreateTakenTestCar();
        _createInvalidTestCar = CarsTestData.CreateInvalidTestCar();
        _updateValidTestCar = CarsTestData.UpdateValidTestCar();
        _updateTakenTestCar = CarsTestData.UpdateTakenTestCar();
        _updateInvalidTestCar = CarsTestData.UpdateInvalidTestCar();

        var nonExistentCarId = CarsTestData.NonExistentTestCarId();

        _createCarRoute = "cars";
        _updateExistingCarRoute = $"cars/{_updateTakenTestCar.Id.Value}";
        _updateNonExistingCarRoute = $"cars/{nonExistentCarId}";
        _deleteExistingCarRoute = $"cars/{_createTakenTestCar.Id.Value}";
        _deleteNonExistingCarRoute = $"cars/{nonExistentCarId}";
    }
    
    public async Task InitializeAsync()
    {
        await Context.Cars.AddAsync(_createTakenTestCar);
        await Context.Cars.AddAsync(_updateTakenTestCar);
        
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Cars.RemoveRange(Context.Cars);
        
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }
    
    [Fact]
    public async Task ShouldCreateCar()
    {
        // Arrange
        var request = new CreateCarDto(
            _createValidTestCar.Vin,
            _createValidTestCar.Brand,
            _createValidTestCar.Model,
            _createValidTestCar.Price,
            _createValidTestCar.FuelType,
            _createValidTestCar.IsAvailable);

        // Act
        var response = await Client.PostAsJsonAsync(_createCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseDto = await response.Content.ReadFromJsonAsync<CarDto>(JsonOptions);
        responseDto.Should().NotBeNull();

        var dbCar = await Context.Cars.FirstOrDefaultAsync(x => x.Id == new CarId(responseDto!.Id));
        dbCar.Should().NotBeNull();
        dbCar!.Vin.Should().Be(_createValidTestCar.Vin);
        dbCar.Brand.Should().Be(_createValidTestCar.Brand);
        dbCar.Model.Should().Be(_createValidTestCar.Model);
        dbCar.Price.Should().Be(_createValidTestCar.Price);
        dbCar.FuelType.Should().Be(_createValidTestCar.FuelType);
        dbCar.IsAvailable.Should().Be(_createValidTestCar.IsAvailable);
    }

    [Fact]
    public async Task ShouldNotCreateCarBecauseVinIsAlreadyTaken()
    {
        // Arrange
        var request = new CreateCarDto(
            _createTakenTestCar.Vin,
            _createTakenTestCar.Brand,
            _createTakenTestCar.Model,
            _createTakenTestCar.Price,
            _createTakenTestCar.FuelType,
            _createTakenTestCar.IsAvailable);

        // Act
        var response = await Client.PostAsJsonAsync(_createCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbCar = await Context.Cars.SingleAsync(x => x.Vin == _createTakenTestCar.Vin);
        dbCar.Id.Value.Should().Be(_createTakenTestCar.Id.Value);
    }

    [Fact]
    public async Task ShouldNotCreateCarBecauseRequestIsInvalid()
    {
        // Arrange
        var carsInDb = await Context.Cars.CountAsync();
        var request = new CreateCarDto(
            _createInvalidTestCar.Vin,
            _createInvalidTestCar.Brand,
            _createInvalidTestCar.Model,
            _createInvalidTestCar.Price,
            _createInvalidTestCar.FuelType,
            _createInvalidTestCar.IsAvailable);

        // Act
        var response = await Client.PostAsJsonAsync(_createCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbCarCount = await Context.Cars.CountAsync();
        dbCarCount.Should().Be(carsInDb);
    }

    [Fact]
    public async Task ShouldUpdateCar()
    {
        // Arrange
        var request = new UpdateCarDto(
            _updateValidTestCar.Vin,
            _updateValidTestCar.Brand,
            _updateValidTestCar.Model,
            _updateValidTestCar.Price,
            _updateValidTestCar.FuelType,
            _updateValidTestCar.IsAvailable);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseDto = await response.Content.ReadFromJsonAsync<CarDto>(JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_updateTakenTestCar.Id.Value);

        var dbCar = await Context.Cars.FirstOrDefaultAsync(x => x.Id == _updateTakenTestCar.Id);
        dbCar.Should().NotBeNull();
        dbCar!.Vin.Should().Be(_updateValidTestCar.Vin);
        dbCar.Brand.Should().Be(_updateValidTestCar.Brand);
        dbCar.Model.Should().Be(_updateValidTestCar.Model);
        dbCar.Price.Should().Be(_updateValidTestCar.Price);
        dbCar.FuelType.Should().Be(_updateValidTestCar.FuelType);
        dbCar.IsAvailable.Should().Be(_updateValidTestCar.IsAvailable);
    }

    [Fact]
    public async Task ShouldNotUpdateCarBecauseVinAlreadyTaken()
    {
        var request = new UpdateCarDto(
            _createTakenTestCar.Vin,
            _updateValidTestCar.Brand,
            _updateValidTestCar.Model,
            _updateValidTestCar.Price,
            _updateValidTestCar.FuelType,
            _updateValidTestCar.IsAvailable);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbCar = await Context.Cars.SingleAsync(x => x.Id == _updateTakenTestCar.Id);
        dbCar.Id.Value.Should().Be(_updateTakenTestCar.Id.Value);
        dbCar.Vin.Should().Be(_updateTakenTestCar.Vin);
        dbCar.Brand.Should().Be(_updateTakenTestCar.Brand);
        dbCar.Model.Should().Be(_updateTakenTestCar.Model);
        dbCar.Price.Should().Be(_updateTakenTestCar.Price);
    }

    [Fact]
    public async Task ShouldNotUpdateCarBecauseRequestIsInvalid()
    {
        // Arrange
        var request = new UpdateCarDto(
            _updateInvalidTestCar.Vin,
            _updateInvalidTestCar.Brand,
            _updateInvalidTestCar.Model,
            _updateInvalidTestCar.Price,
            _updateInvalidTestCar.FuelType,
            _updateInvalidTestCar.IsAvailable);

        // Act
        var response = await Client.PutAsJsonAsync(_updateExistingCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbCar = await Context.Cars.SingleAsync(x => x.Id == _updateTakenTestCar.Id);
        dbCar.Vin.Should().Be(_updateTakenTestCar.Vin);
        dbCar.Price.Should().Be(_updateTakenTestCar.Price);
    }

    [Fact]
    public async Task ShouldNotUpdateCarBecauseCarNotFound()
    {
        // Arrange
        var request = new UpdateCarDto(
            _updateValidTestCar.Vin,
            _updateValidTestCar.Brand,
            _updateValidTestCar.Model,
            _updateValidTestCar.Price,
            _updateValidTestCar.FuelType,
            _updateValidTestCar.IsAvailable);

        // Act
        var response = await Client.PutAsJsonAsync(_updateNonExistingCarRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteCar()
    {
        // Act
        var response = await Client.DeleteAsync(_deleteExistingCarRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbCar = await Context.Cars.FirstOrDefaultAsync(x => x.Id == _createTakenTestCar.Id);
        dbCar.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteCarBecauseCarNotFound()
    {
        // Arrange
        var dbCarsCount = await Context.Cars.CountAsync();

        // Act
        var response = await Client.DeleteAsync(_deleteNonExistingCarRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var dbCarCount = await Context.Cars.CountAsync();
        dbCarCount.Should().Be(dbCarsCount);
    }
}