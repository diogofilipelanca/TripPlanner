﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TripPlanner.Client.Models;

namespace TripPlanner.Controller {
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase {
        private readonly IWebHostEnvironment env;

        public TripsController(IWebHostEnvironment _env) {
            env = _env;
        }

        [HttpGet]
        public async Task<List<Trip>> GetAllTrips() {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");
            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    return JsonSerializer.Deserialize<List<Trip>>(existingJson) ?? new();
                }
            }
            return new();
        }

        [HttpGet("{id}")]
        public async Task<Trip> GetByIdTrips(string id) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");
            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);

                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    var trips = JsonSerializer.Deserialize<List<Trip>>(existingJson);
                    return trips?.Where(trip => trip.Id == id).FirstOrDefault() ?? new();
                }
            }
            return new();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] Trip trip) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");
            Directory.CreateDirectory(Path.GetDirectoryName(filepath)!);

            List<Trip> tripList = new List<Trip>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    tripList = JsonSerializer.Deserialize<List<Trip>>(existingJson) ?? new();
                }
            }

            tripList.Add(trip);

            var newJson = JsonSerializer.Serialize(tripList, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(filepath, newJson);

            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTrip(string id, [FromBody] Trip _trip) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");

            List<Trip> tripList = new List<Trip>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    tripList = JsonSerializer.Deserialize<List<Trip>>(existingJson) ?? new();
                    var trip = tripList.Where(c => c.Id == id).FirstOrDefault()!;
                    tripList.Remove(trip);
                    tripList.Add(_trip);

                    var newJson = JsonSerializer.Serialize(tripList, new JsonSerializerOptions { WriteIndented = true });
                    await System.IO.File.WriteAllTextAsync(filepath, newJson);
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(string id) {
            var filepath = Path.Combine(env.ContentRootPath, "Data", "Trips.json");

            List<Trip> tripList = new List<Trip>();

            if (System.IO.File.Exists(filepath)) {
                var existingJson = await System.IO.File.ReadAllTextAsync(filepath);
                if (!string.IsNullOrWhiteSpace(existingJson)) {
                    tripList = JsonSerializer.Deserialize<List<Trip>>(existingJson) ?? new();
                    var city = tripList.Where(c => c.Id == id).FirstOrDefault()!;
                    tripList.Remove(city);

                    var newJson = JsonSerializer.Serialize(tripList, new JsonSerializerOptions { WriteIndented = true });
                    await System.IO.File.WriteAllTextAsync(filepath, newJson);
                }
            }
            return Ok();
        }
    }
}
