namespace MLS_UI.Models
{
    public class DetectionDto
    {
        public NoteDto Patient { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int TriggerCount { get; set; }
        public List<string> TriggerList { get; set; }
        public string RiskExpected { get; set; }
    }
}
