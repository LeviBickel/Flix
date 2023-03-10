using System.ComponentModel.DataAnnotations;

namespace Flix.Models
{
    public class AdminSettings
    {
        public int ID { get; set; }
        public bool Scheduled { get; set; }
        
        [Display(Name ="Maintenance Date")]
        public string MaintenanceDate { get; set; }
        
        [Display(Name = "Maintenance Time")]
        public string MaintenanceTime { get; set; }
        public static string MaintenaceInfo { get; set; }
        public static string MaintenaceTimeInfo { get; set; }
        public static bool ScheduledBool { get; set; }
        public bool useExistingDirectory { get; set; }
        public int directoryScannerInterval { get; set; }
        public string directoryPath { get; set; }
    }
}
