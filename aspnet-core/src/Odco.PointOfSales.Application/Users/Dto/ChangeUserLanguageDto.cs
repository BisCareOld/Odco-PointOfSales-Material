using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}