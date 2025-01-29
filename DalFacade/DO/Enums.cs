namespace DO
{
    public class Enums
    {
        // Types of call assignment completion statuses.
        public enum TreatmentStatus
        {
            CompletedOnTime,
            CanceledByVolunteer,
            CanceledByManager,
            Expired
        }

        // Different types of calls.
        public enum CallType
        {
            EmotionalSupport,
            FamilySupport,
            ProfessionalConsultation,
            Emergency
        }

        // Types of volunteer roles.
        public enum Role
        {
            Volunteer,
            Manager
        }

        // Types of distances.
        public enum DistanceType
        {
            Air,
            Walking,
            Car
        }
    }
}
