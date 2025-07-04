﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TripPlanner.Client.Models;

namespace TripPlanner.Controller {
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase {
        private readonly IWebHostEnvironment env;

        public CitiesController(IWebHostEnvironment _env) {
            env = _env;
        }

        [HttpGet]
        public async Task<List<City>> GetAllCities() {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Cities.json");
            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    return JsonSerializer.Deserialize<List<City>>(existingJson) ?? new();
                }
            }
            return new();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] City city) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Cities.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filepath)!);

            List<City> cityList = new List<City>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    cityList = JsonSerializer.Deserialize<List<City>>(existingJson) ?? new();
                }
            }

            cityList.Add(city);

            var newJson = JsonSerializer.Serialize(cityList, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(filepath, newJson);

            return Ok();
        }

        [HttpGet("{name}")]
        public async Task<City> GetByIdTrips(string name) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Cities.json");
            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);

                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    var cities = JsonSerializer.Deserialize<List<City>>(existingJson);
                    return cities?.Where(city => city.Name == name).FirstOrDefault() ?? new();
                }
            }
            return new();
        }

        [HttpPatch("{name}")]
        public async Task<IActionResult> UpdateTrip(string name, [FromBody] City _city) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Cities.json");

            List<City> cityList = new List<City>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    cityList = JsonSerializer.Deserialize<List<City>>(existingJson) ?? new();
                    var city = cityList.Where(c => c.Name == name).FirstOrDefault()!;
                    cityList.Remove(city);
                    cityList.Add(_city);

                    var newJson = JsonSerializer.Serialize(cityList, new JsonSerializerOptions { WriteIndented = true });
                    await System.IO.File.WriteAllTextAsync(filepath, newJson);

                    var tripsfilepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");
                    List<Trip> tripList = new List<Trip>();

                    if (System.IO.File.Exists(tripsfilepath)) {
                        var tripExistingJson = await System.IO.File.ReadAllTextAsync(tripsfilepath);
                        if (!string.IsNullOrWhiteSpace(tripExistingJson)) {
                            tripList = JsonSerializer.Deserialize<List<Trip>>(tripExistingJson) ?? new();

                            var tripsToChangeOrigin = tripList.Where(trip => trip.Origin == name).ToList();
                            tripsToChangeOrigin.ForEach(trip => trip.Origin = _city.Name);

                            var tripsToChangeDestination = tripList.Where(trip => trip.Destination == name).ToList();
                            tripsToChangeDestination.ForEach(trip => trip.Destination = _city.Name);

                            var tripNewJson = JsonSerializer.Serialize(tripList, new JsonSerializerOptions { WriteIndented = true });
                            await System.IO.File.WriteAllTextAsync(tripsfilepath, tripNewJson);
                        }
                    }
                }
            }
            return Ok();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteCity(string name) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Cities.json");
            var tripsfilepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");

            List<City> cityList = new List<City>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    cityList = JsonSerializer.Deserialize<List<City>>(existingJson) ?? new();
                    var city = cityList.Where(c => c.Name == name).FirstOrDefault()!;
                    cityList.Remove(city);

                    var newJson = JsonSerializer.Serialize(cityList, new JsonSerializerOptions { WriteIndented = true });
                    await System.IO.File.WriteAllTextAsync(filepath, newJson);

                    if (System.IO.File.Exists(tripsfilepath)) {
                        var tripsJson = await System.IO.File.ReadAllTextAsync(tripsfilepath);
                        var tripsList = JsonSerializer.Deserialize<List<Trip>>(tripsJson) ?? new();
                        tripsList.RemoveAll(t => t.Origin == name || t.Destination == name);

                        var newtripsJson = JsonSerializer.Serialize(tripsList, new JsonSerializerOptions { WriteIndented = true });
                        await System.IO.File.WriteAllTextAsync(tripsfilepath, newtripsJson);
                    }
                }
            }
            return Ok();
        }
    }
}
