using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminClient.Models;
using Microsoft.Extensions.Configuration;

namespace AdminClient.Services
{
    /// <summary>
    /// Service for handling all REST API communications with the backend.
    /// Uses HttpClient to make REST calls and handles serialization/deserialization of DTOs.
    /// </summary>
    public class ApiService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private bool _disposed;

        public ApiService(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:8080";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        // Region Operations
        public async Task<Region> GetRegionAsync(string regionId)
        {
            return await _httpClient.GetFromJsonAsync<Region>($"/regions/{regionId}")
                ?? throw new Exception("Region not found");
        }

        public async Task<List<Organization>> GetOrganizationsForRegionAsync(string regionId)
        {
            return await _httpClient.GetFromJsonAsync<List<Organization>>($"/regions/{regionId}/organizations")
                ?? new List<Organization>();
        }

        public async Task<Organization> CreateOrganizationAsync(string regionId, Organization org)
        {
            var response = await _httpClient.PostAsJsonAsync($"/regions/{regionId}/organizations", org);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Organization>()
                ?? throw new Exception("Failed to create organization");
        }

        public async Task<Organization> GetOrganizationAsync(long orgId)
        {
            return await _httpClient.GetFromJsonAsync<Organization>($"/organizations/{orgId}")
                ?? throw new Exception("Organization not found");
        }

        // Todo: using hard-coded regionId "us" for now
        public async Task DeleteOrganizationAsync(long orgId)
        {
            var response = await _httpClient.DeleteAsync($"regions/us/organizations/{orgId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Program>> GetProgramsForOrganizationAsync(long orgId)
        {
            return await _httpClient.GetFromJsonAsync<List<Program>>($"/organizations/{orgId}/programs")
                ?? new List<Program>();
        }

        public async Task<Program> CreateProgramAsync(long orgId, Program program)
        {
            var response = await _httpClient.PostAsJsonAsync($"/organizations/{orgId}/programs", program);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Program>()
                ?? throw new Exception("Failed to create program");
        }

        // Program Operations
        public async Task<Program> GetProgramAsync(long programId)
        {
            return await _httpClient.GetFromJsonAsync<Program>($"/programs/{programId}")
                ?? throw new Exception("Program not found");
        }

        public async Task<List<OperatingUnit>> GetOperatingUnitsForProgramAsync(long programId)
        {
            return await _httpClient.GetFromJsonAsync<List<OperatingUnit>>($"/programs/{programId}/operating-units")
                ?? new List<OperatingUnit>();
        }

        public async Task<OperatingUnit> CreateOperatingUnitAsync(long programId, OperatingUnit ou)
        {
            var response = await _httpClient.PostAsJsonAsync($"/programs/{programId}/operating-units", ou);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OperatingUnit>()
                ?? throw new Exception("Failed to create operating unit");
        }

        // Bundle and Activity Operations
        public async Task<List<BundleDefinition>> GetBundleDefinitionsForProgramAsync(long programId)
        {
            var response = await _httpClient.GetAsync($"/programs/{programId}/bundle-definitions");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<BundleDefinition>>();
        }

        public async Task<BundleDefinition> CreateBundleDefinitionAsync(long programId, BundleDefinition bundle)
        {
            var response = await _httpClient.PostAsJsonAsync($"/programs/{programId}/bundle-definitions", bundle);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BundleDefinition>()
                ?? throw new Exception("Failed to create bundle definition");
        }

        public async Task<ActivityDefinition> CreateActivityDefinitionAsync(long programId, ActivityDefinition activity)
        {
            var response = await _httpClient.PostAsJsonAsync($"/programs/{programId}/activity-definitions", activity);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ActivityDefinition>()
                ?? throw new Exception("Failed to create activity definition");
        }

        public async Task AddActivityToBundleAsync(long programId, long bundleId, long activityId)
        {
            var response = await _httpClient.PostAsync(
                $"/programs/{programId}/bundle-definitions/{bundleId}/activity-definitions/{activityId}",
                null);
            response.EnsureSuccessStatusCode();
        }

        // Operating Unit Operations
        public async Task<OperatingUnit> GetOperatingUnitAsync(long ouId)
        {
            return await _httpClient.GetFromJsonAsync<OperatingUnit>($"/operating-units/{ouId}")
                ?? throw new Exception("Operating unit not found");
        }

        public async Task AddHcpToOperatingUnitAsync(long ouId, long userId)
        {
            var response = await _httpClient.PostAsync($"/operating-units/{ouId}/hcps/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddSubjectToOperatingUnitAsync(long ouId, long userId)
        {
            var response = await _httpClient.PostAsync($"/operating-units/{ouId}/subjects/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        // User Operations
        public async Task<List<User>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("/users")
                ?? new List<User>();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/users", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>()
                ?? throw new Exception("Failed to create user");
        }

        public async Task<User> GetUserAsync(long userId)
        {
            return await _httpClient.GetFromJsonAsync<User>($"/users/{userId}")
                ?? throw new Exception("User not found");
        }

        // IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
                _disposed = true;
            }
        }
    }
}