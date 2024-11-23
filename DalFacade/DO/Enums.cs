
namespace DO
{
    public class Enums
    {
        /// <summary>
        /// סוגי סיום הטיפול בהקצאת הקריאה.
        /// </summary>
        /// <remarks>
        /// - <see cref="CompletedOnTime"/>: הקריאה טופלה בזמן.
        /// - <see cref="CanceledByVolunteer"/>: המתנדב ביטל את הטיפול.
        /// - <see cref="CanceledBymanager"/>: המנהל ביטל את ההקצאה.
        /// - <see cref="Expired"/>: הקריאה בוטלה עקב פג תוקף (לא טופלה בזמן).
        /// </remarks>
        public enum TreatmentStatus
        {
            CompletedOnTime,
            CanceledByVolunteer,
            CanceledBymanager,
            Expired
        }

        /// <summary>
        /// סוגי הקריאות השונות.
        /// </summary>
        /// <remarks>
        /// - <see cref="FoodPreparation"/>: הכנת אוכל.
        /// - <see cref="FoodTransport"/>: שינוע האוכל.
        /// </remarks>
        public enum CallType
        {
            EmotionalSupport,
            FamilySupport,
            ProfessionalConsultation,
            Emergency
        }

        /// <summary>
        /// סוגי תפקידים של המתנדב.
        /// </summary>
        /// <remarks>
        /// - <see cref="manager"/>: מנהל.
        /// - <see cref="Volunteer"/>: מתנדב.
        /// </remarks>
        public enum Role
        {
            manager,
            Volunteer
        }

        /// <summary>
        /// סוגי מרחקים.
        /// </summary>
        /// <remarks>
        /// - <see cref="Air"/>: מרחק אווירי.
        /// - <see cref="Walking"/>: מרחק הליכה.
        /// - <see cref="Car"/>: מרחק נסיעה ברכב.
        /// </remarks>
        public enum DistanceType
        {
            Air,
            Walking,
            Car
        }
    }
}
   
