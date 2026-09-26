using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[MasterReligions]
    /// </summary>
    [Table("MasterReligions")]
    public class MasterReligion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? NativeName { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Database Table Mapping: [dbo].[MasterMotherTongues]
    /// </summary>
    [Table("MasterMotherTongues")]
    public class MasterMotherTongue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? NativeName { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Database Table Mapping: [dbo].[MasterEducations]
    /// </summary>
    [Table("MasterEducations")]
    public class MasterEducation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string DegreeName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Database Table Mapping: [dbo].[MasterOccupations]
    /// </summary>
    [Table("MasterOccupations")]
    public class MasterOccupation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Sector { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Database Table Mapping: [dbo].[MasterIncomeRanges]
    /// </summary>
    [Table("MasterIncomeRanges")]
    public class MasterIncomeRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string RangeText { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// Database Table Mapping: [dbo].[MasterLocations]
    /// </summary>
    [Table("MasterLocations")]
    public class MasterLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CityName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string StateName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Country { get; set; } = "India";

        public bool IsPopular { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }
}
