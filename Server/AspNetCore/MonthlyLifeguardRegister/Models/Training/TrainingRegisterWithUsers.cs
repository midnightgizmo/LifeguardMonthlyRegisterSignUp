using MonthlyLifeguardRegister.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonthlyLifeguardRegister.Models.Training
{
    public class TrainingRegisterWithUsers : TrainingRegister
    {
        /// <summary>
        /// Limited user details (does not contain users password)
        /// </summary>
        public List<JwtUser> usersInTrainingSessionArray { get; set; } = new List<JwtUser>();

        /// <summary>
        /// Full users details. would not want this sent back to the client because it contains users password
        /// </summary>
        [JsonIgnore]
        public List<UserFullDetails> usersInTrainingSessionArray_FullDetails { get; set; } = new List<UserFullDetails>();


        /// <summary>
        /// copy all the users in the <see cref="usersInTrainingSessionArray_FullDetails"/> and put them into the <see cref="usersInTrainingSessionArray"/> array
        /// </summary>
        public void ConvertFullDetailsToLimitedDetails()
        {
            // go through each user in the usersInTrainingSessionArray_FullDetails list and add them to the usersInTrainingSessionArray.
            // We do this because usersInTrainingSessionArray holds less information than usersInTrainingSessionArray_FullDetails.
            // e.g. it does not contain the password. This is usefull when we want to send information back to the client but
            // don't want them to be aware of the password
            foreach (UserFullDetails usersFullDetails in this.usersInTrainingSessionArray_FullDetails)
                this.usersInTrainingSessionArray.Add(usersFullDetails);
        }
    }
}
