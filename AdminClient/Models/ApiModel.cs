using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    // Base message content interface for type discrimination
    [JsonDerivedType(typeof(SubjectsListUpdate), typeDiscriminator: "SUBJECTS_LIST_UPDATE")]
    [JsonDerivedType(typeof(BundlesListUpdate), typeDiscriminator: "BUNDLES_LIST_UPDATE")]
    [JsonDerivedType(typeof(Assignment), typeDiscriminator: "ASSIGNMENT")]
    [JsonDerivedType(typeof(BatchAssignment), typeDiscriminator: "BATCH_ASSIGNMENT")]
    public interface IMessageContent { }

    public class Region
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "US";

        [JsonPropertyName("organizations")]
        public List<Organization> Organizations { get; set; } = new();
    }

    public class Organization
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("programs")]
        public List<Program> Programs { get; set; } = new();
    }

    public class Program
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("organization")]
        public Organization Organization { get; set; }

        [JsonPropertyName("operatingUnits")]
        public List<OperatingUnit> OperatingUnits { get; set; } = new();
    }

    public class User
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("role")]
        public UserRole Role { get; set; }

        [JsonPropertyName("organization")]
        public Organization Organization { get; set; }
    }

    public enum UserRole
    {
        ORGANIZATION_ADMIN,
        PROGRAM_ADMIN,
        OPERATING_UNIT_ADMIN,
        HCP,
        SUBJECT
    }

    public class OperatingUnit
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("program")]
        public Program Program { get; set; }

        // Note: HCPs and Subjects are marked JsonIgnore in Java, so we don't include them here
    }

    public class ActivityDefinition
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("instructions")]
        public string Instructions { get; set; }

        [JsonPropertyName("hcpOperated")]
        public bool HcpOperated { get; set; }

        [JsonPropertyName("program")]
        public Program Program { get; set; }
    }

    public class BundleDefinition
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BundleStatus Status { get; set; } = BundleStatus.DRAFT;

        [JsonPropertyName("program")]
        public Program Program { get; set; }

        [JsonPropertyName("activities")]
        public List<ActivityDefinition> Activities { get; set; } = new();
    }

    public enum BundleStatus
    {
        DRAFT,
        APPROVED
    }

    // Message-related DTOs
    public class SubjectsListUpdate : IMessageContent
    {
        [JsonPropertyName("subjectIds")]
        public List<long> SubjectIds { get; set; } = new();
    }

    public class BundlesListUpdate : IMessageContent
    {
        [JsonPropertyName("bundleIds")]
        public List<long> BundleIds { get; set; } = new();
    }

    public class Assignment : IMessageContent
    {
        [JsonPropertyName("subjectId")]
        public long SubjectId { get; set; }

        [JsonPropertyName("bundleIds")]
        public List<long> BundleIds { get; set; } = new();
    }

    public class BatchAssignment : IMessageContent
    {
        [JsonPropertyName("subjectIds")]
        public List<long> SubjectIds { get; set; } = new();

        [JsonPropertyName("bundleIds")]
        public List<long> BundleIds { get; set; } = new();
    }

    public abstract class Message
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("fromOperatingUnit")]
        public OperatingUnit FromOperatingUnit { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("processed")]
        public bool Processed { get; set; }
    }

    public class HcpMessage : Message
    {
        // Inherits all properties from Message
    }

    public class SubjectMessage : Message
    {
        [JsonPropertyName("destinationSubject")]
        public User DestinationSubject { get; set; }
    }

    public class PersonalQueue
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();
    }
}
