using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.Training
{
    public class TrainingRegister
    {

        /// <summary>
        /// Id of the training register in the database
        /// </summary>
        public int id { get; set; } = 0;

        /// <summary>
        /// Idicates when this training should take place.
        /// Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
        /// </summary>
        public long dateTimeOfTraining { get; set; } = 0;

        /// <summary>
        /// Indicates the earilest date/time when this register appears to users.
        /// Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
        /// </summary>
        public long dateTimeWhenRegisterIsActive { get; set; } = 0;

        /// <summary>
        /// Indicatees the earlyist date/time users can start adding there name to this register
        /// Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
        /// </summary>
        public long dateTimeFromWhenRegisterCanBeUsed{ get; set; } = 0;

        /// <summary>
        /// The maxium number of people allowed on this register before it becomes full
        /// </summary>
        public int maxNoCandidatesAllowed { get; set; } = 0;
    }
}
