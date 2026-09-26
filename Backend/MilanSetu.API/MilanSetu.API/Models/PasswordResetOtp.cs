using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilanSetu.API.Models
{
    /// <summary>
    /// Database Table Mapping: [dbo].[PasswordResetOtps]
    /// Stores short-lived OTP verification codes for Forgot Password authentication workflows.
    /// </summary>
    [Table("PasswordResetOtps")]
    public class PasswordResetOtp
    {
        /// <summary>
        /// Column: Id (INT, Primary Key, Identity)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Column: Identifier (NVARCHAR(150), NOT NULL) - User Email or Mobile
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Column: OtpCode (NVARCHAR(6), NOT NULL) - 6-digit numeric verification code
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        /// <summary>
        /// Column: ExpiresAt (DATETIME2, NOT NULL) - Expiration timestamp (e.g. 10 mins from creation)
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Column: IsUsed (BIT, NOT NULL, Default: 0) - Flag indicating OTP consumption
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Column: CreatedAt (DATETIME2, NOT NULL)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
