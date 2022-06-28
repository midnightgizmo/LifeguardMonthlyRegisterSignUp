import axios from 'axios';
import {loginAuthentication} from '@/models/authentication';
import {Register} from '@/models/register';
import { RegisterWithUsers } from './registerWithUsers';
import { user } from './user';
import { LungType} from './ManikinLungs/LungType';
import { LungUserGiven } from './ManikinLungs/LungUserGiven';

export class serverAuthentication
{
    public async logIn(surname : string, dateOfBirth_Day : string, dateOfBirth_Month: string, dateOfBirth_Year : string ) : Promise<loginAuthentication>
    {
        let server = new Server();
        let response : Response;

        try
        {
            //let data = new Data();
            //data.data = JSON.stringify({userName : surname, userPassword : dateOfBirth});
            let data = {userName : surname, day : dateOfBirth_Day, month : dateOfBirth_Month, year : dateOfBirth_Year };

            // send the data off to the server and wait for a response
            response = await server.sendPostRequestToServer('/Authentication/Login.php',data);
            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    let loginData :  loginAuthentication = new loginAuthentication();

                    Object.assign(loginData, response.data);
                    // send back true to indicate the file was deleted
                    success(loginData);
                });
            }
            else
            {
                // if we recieved a 403 it means we were sucesfull in comunicating with the server
                // but the login details were incorrect
                if(response.satusCode == 403)
                {
                    return new Promise((success, fail) =>
                    {
                        let loginData :  loginAuthentication = new loginAuthentication();

                        Object.assign(loginData, response.errorData);
                        // send back true to indicate the file was deleted
                        success(loginData);
                    });
                }
                // unknonw error
                else
                {
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                }
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }
    }

    public async adminLogin(username : string, password : string) : Promise<loginAuthentication>
    {
        let server = new Server();
        let response : Response;

        try
        {
            let data = {userName : username, password :password };

            // send the data off to the server and wait for a response
            response = await server.sendPostRequestToServer('/admin/Authentication/Login.php',data);

            
            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    let loginData :  loginAuthentication = new loginAuthentication();

                    Object.assign(loginData, response.data);
                    // send back true to indicate the file was deleted
                    success(loginData);
                });
            }
            else
            {
                // if we recieved a 403 it means we were sucesfull in comunicating with the server
                // but the login details were incorrect
                if(response.satusCode == 403)
                {
                    return new Promise((success, fail) =>
                    {
                        let loginData :  loginAuthentication = new loginAuthentication();

                        Object.assign(loginData, response.errorData);
                        
                        success(loginData);
                    });
                }
                // unknonw error
                else
                {
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                }
            }

        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }
    }
}

export class ajaxRegister
{
    /** gets a list of avalable register from the server */
    public async getAvalableRegisters() : Promise<Register[]>
    {
        let server = new Server();
        let response : Response;

        try
        {
            response = await server.sendGetRequestToServer("/Training/TrainingRegister/getUpcomingTrainingRegisters.php");
            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    let registers :  Register[] =[];
                    // we should have recieved an array of registers
                    if(Array.isArray(response.data) == true)
                    {// it is an array
                        if(response.data.length > 0)
                        {
                            // go through each array item
                            (response.data as Array<any>).forEach((value) =>
                            {// convert the current item in the array
                            // to a Register object 
                                let aRegister = new Register;
                                aRegister.id = value.id;
                                aRegister.dateTimeOfTraining_UnixTimeStamp = value.dateTimeOfTraining;
                                aRegister.dateTimeOfWhenRegisterIsVisable = value.dateTimeWhenRegisterIsActive;
                                aRegister.dateTimeFromWhenCanBookOntoSession = value.dateTimeFromWhenRegisterCanBeUsed;
                                //Object.assign(aRegister, value);
                                // Object.asssign does not set the place up properly
                                // so we need to call the inishalize method to finish
                                // setting up the class.
                                //aRegister.inishalize();

                                // add the register to the registers list
                                registers.push(aRegister);
                            });
                        }
                    }
                                        
                    // we should now have a list of registers
                    success(registers);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }
    }



    public async getRegistesInMonth(year : number, month : number): Promise<RegisterWithUsers[]>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {year : year, month: month};

        let registersArray : RegisterWithUsers[] = [];
        try
        {
            response = await server.sendPostRequestToServer("/Training/TrainingRegister/getAllRegistersInMonth.php",jsonData);

            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    // we should have recieved an array of registers
                    if(Array.isArray(response.data) == true)
                    {// it is an array
                        
                        if(response.data.length > 0)
                        {
                            // go through each array item and get each register
                            (response.data as Array<any>).forEach((value) =>
                            {
                                // create an instance of RegisterWithUsers
                                // that will hold all the data for this register wtih any user that are in it
                                let aRegister = new RegisterWithUsers();
                                
                                // copy over the values that were sent from the server
                                aRegister.id = value.id;
                                aRegister.dateTimeOfTraining_UnixTimeStamp = value.dateTimeOfTraining;
                                aRegister.dateTimeOfWhenRegisterIsVisable = value.dateTimeWhenRegisterIsActive;
                                aRegister.dateTimeFromWhenCanBookOntoSession = value.dateTimeFromWhenRegisterCanBeUsed;
                                aRegister.maxNoCandidatesAllowed = value.maxNoCandidatesAllowed;
                                

                                // get all the user in the register
                                if(Array.isArray(value.usersInTrainingSessionArray) == true)
                                {// it is an array

                                    // check if there is anything in the array
                                    if(value.usersInTrainingSessionArray.length > 0)
                                    {
                                        (value.usersInTrainingSessionArray as Array<any>).forEach((eachUser) =>
                                        {
                                            let aUser = new user();
                                            aUser.id = eachUser.id;
                                            aUser.firstName = eachUser.firstName;
                                            aUser.surname = eachUser.surname;

                                            // add the user to the registers user list
                                            aRegister.usersInRegister.push(aUser);
                                        });
                                    }
                                }

                                aRegister.inishalize();
                                // add the register to the registers list
                                registersArray.push(aRegister);
                                
                                
                            });
                        }
                    }

                    success(registersArray);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });

            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }
    }

    public async getRegistersBetween(startDate : Date, endDate : Date) : Promise<Register[]>
    {
        let server = new Server();
        let response : Response;
        // will hold an array of registers recieved from the server
        let registersArray : Register[] = [];

        // the data we want to send to the server
        let jsonData = 
        {
            startDateDay : startDate.getDate(), 
            startDateMonth: startDate.getMonth() + 1,
            startDateYear: startDate.getFullYear(),

            endDateDay : endDate.getDate(), 
            endDateMonth: endDate.getMonth() + 1,
            endDateYear: endDate.getFullYear(),
        };

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/getAllRegistersBetween.php",jsonData);

            if(response.wereErrors == false)
            {

                return new Promise((success, fail) =>
                {
                    
                    // we should have recieved an array of registers
                    if(Array.isArray(response.data) == true)
                    {// it is an array
                        
                        if(response.data.length > 0)
                        {
                            
                            // go through each array item and get each register
                            (response.data as Array<any>).forEach((value) =>
                            {

                                // create an instance of Register
                                // that will hold all the data for this register
                                let aRegister = new Register();
                                
                                // copy over the values that were sent from the server
                                aRegister.id = value.id;
                                aRegister.dateTimeOfTraining_UnixTimeStamp = value.dateTimeOfTraining;
                                aRegister.dateTimeOfWhenRegisterIsVisable = value.dateTimeWhenRegisterIsActive;
                                aRegister.dateTimeFromWhenCanBookOntoSession = value.dateTimeFromWhenRegisterCanBeUsed;
                                aRegister.inishalize();
                                // add the register to the registers list
                                registersArray.push(aRegister);

                            });
                            
                        }

                    
                    }

                    success(registersArray);

                });


            }
            
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });

            }
            

        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }

        
    }

    
    /**
     * Adds the currently logged in user to the register (if passed in userID is not same as JWT user id, it will not add them)
     * @param userID user to add to register (must be the same as the currently logged in ueserID for the JWT)
     * @param registerID The register to add the user too
     * @returns the user that was added to the register if sucsefull
     */
    public async addUserToRegister(userID : number, registerID : number) : Promise<user>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userID : userID, registerID: registerID};

        try
        {
            response = await server.sendPostRequestToServer("/Training/TrainingRegister/addUserToRegister.php",jsonData);
            
            if(response.wereErrors == false)
            {
                let aUser = new user();

                // check if we were able to add user to the register.
                // if id was set to -1 the user was not added
                if(response.data.id == "-1")
                    aUser.id = -1;
                else
                {// user was added to database on server
                    aUser.id = response.data.id;
                    aUser.firstName = response.data.firstName;
                    aUser.surname = response.data.surname;
                }

                return new Promise((success,fail) =>
                {
                    success(aUser);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }



    }

    /**
     * Can only be called from the admin user
     * @param userID id of user to add to register
     * @param registerID id or register user should be added to
     * @returns returns true if user added to register, else false
     */
    public async adminAddUserToRegister(userID: number, registerID : number) :Promise<boolean>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userID : userID, registerID: registerID};

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/addUserToRegister.php",jsonData);
            
            if(response.wereErrors == false)
            {
                if((<boolean>response.data) == true)
                    return new Promise((sucsess,fail)=>{ sucsess(true)}); 
                else
                    return new Promise((sucsess,fail)=>{ sucsess(false)}); 
            }
            else
            {
                return new Promise((sucsess,fail)=>{ sucsess(false)}); 
            }
        }
        catch
        {
            return new Promise((sucsess,fail)=>{ fail()});
        }
    }

    
    /**
     * Can only be called from the admin user
     * @param userID id of user to add to register
     * @param registerID id or register user should be added to
     * @returns returns true if user remove from register, else false
     */
     public async adminRemoveUserFromRegister(userID: number, registerID : number) :Promise<boolean>
     {
         let server = new Server();
         let response : Response;
 
         let jsonData = {userID : userID, registerID: registerID};
 
         try
         {
             response = await server.sendPostRequestToServer("/admin/TrainingRegister/removeUserFromRegister.php",jsonData);
             
             if(response.wereErrors == false)
             {
                if((<boolean>response.data) == true)
                    return new Promise((sucsess,fail)=>{ sucsess(true)}); 
                else
                    return new Promise((sucsess,fail)=>{ sucsess(false)}); 
             }
             else
             {
                 return new Promise((sucsess,fail)=>{ sucsess(false)}); 
             }
         }
         catch
         {
             return new Promise((sucsess,fail)=>{ fail()});
         }
     }

    /**
     * 
     * @param registerID the id of the register to look for
     * @returns The register recieved from the server
     */
    public async getRegister(registerID : number) : Promise<Register>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {registerID : registerID};

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/getRegister.php",jsonData);
            
            if(response.wereErrors == false)
            {
                // if the id has been set to -1, it means there was a problem on the server
                // when trying to get the register
                if(response.data.id == -1)
                {
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                }
                // no problems found, get the data
                else
                {
                    let aRegister : Register = new Register();

                    // copy the values sent from the server into the register object
                    aRegister.id = response.data.id;
                    aRegister.dateTimeOfTraining_UnixTimeStamp = response.data.dateTimeOfTraining;
                    aRegister.dateTimeOfWhenRegisterIsVisable = response.data.dateTimeWhenRegisterIsActive;
                    aRegister.dateTimeFromWhenCanBookOntoSession = response.data.dateTimeFromWhenRegisterCanBeUsed;
                    aRegister.maxNoCandidatesAllowed = response.data.maxNoCandidatesAllowed;
                    aRegister.inishalize();

                    return new Promise((success, fail) =>
                    {
                        success(aRegister);
                    });

                }

                
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }

    }


    /**
     * sends a request to the server asking for user to be removed from register
     * @param userID user to remove from register
     * @param registerID register to remove user from
     */
    public async removeUserFromRegister(userID : number, registerID : number) : Promise<user>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userID : userID, registerID: registerID};

        try
        {
            response = await server.sendPostRequestToServer("/Training/TrainingRegister/removeUserFromRegister.php",jsonData);

            if(response.wereErrors == false)
            {
                let aUser = new user();

                // check if we were able to remove user from the register.
                // if id was set to -1 the user was not removed
                if(response.data.id == "-1")
                    aUser.id = -1;
                else
                {// user was removed from database on server
                    aUser.id = response.data.id;
                    aUser.firstName = response.data.firstName;
                    aUser.surname = response.data.surname;
                }

                return new Promise((success,fail) =>
                {
                    success(aUser);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }

        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }



    }




    public async createRegister(dateOfTraining : string, dateWhenUserCanSeeRegister : string, dateWhenUserCanJoinRegister : string, maxNumberOfCandidatesAllowedOnRegister: number) : Promise<boolean>
    {
        let server = new Server();
        let response : Response;
        // will hold an array of registers recieved from the server
        let registersArray : Register[] = [];

        // the data we want to send to the server
        let jsonData = 
        {
            dateOfTraining : dateOfTraining,
            dateWhenUserCanSeeRegister : dateWhenUserCanSeeRegister, 
            dateWhenUserCanJoinRegister: dateWhenUserCanJoinRegister,
            maxNumberOfCandidatesAllowedOnRegister: maxNumberOfCandidatesAllowedOnRegister
        };

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/createRegister.php",jsonData);

            if(response.wereErrors == false)
            {
                
                return new Promise((success,fail) =>
                {
                    success(true);
                });
            }
            else
            {
                return new Promise((success,fail) =>
                {
                    success(false);
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }

    }


    
    public async updateRegister(registerID : number, dateOfTraining : string, dateWhenUserCanSeeRegister : string, dateWhenUserCanJoinRegister : string, maxNumberOfCandidatesAllowedOnRegister: number) : Promise<boolean>
    {
        let server = new Server();
        let response : Response;
        // will hold an array of registers recieved from the server
        let registersArray : Register[] = [];

        // the data we want to send to the server
        let jsonData = 
        {
            registerID : registerID,
            dateOfTraining : dateOfTraining,
            dateWhenUserCanSeeRegister : dateWhenUserCanSeeRegister, 
            dateWhenUserCanJoinRegister: dateWhenUserCanJoinRegister,
            maxNumberOfCandidatesAllowedOnRegister: maxNumberOfCandidatesAllowedOnRegister
        };

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/updateRegister.php",jsonData);

            if(response.wereErrors == false)
            {
                
                return new Promise((success,fail) =>
                {
                    success(true);
                });
            }
            else
            {
                return new Promise((success,fail) =>
                {
                    success(false);
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }

    }

    public async adminRemoveRegister(registerID : number) : Promise<boolean>
    {
        let server = new Server();
        let response : Response;
        

        // the data we want to send to the server
        let jsonData = 
        {
            registerID : registerID
        };

        try
        {
            response = await server.sendPostRequestToServer("/admin/TrainingRegister/deleteRegister.php",jsonData);

            if(response.wereErrors == false)
            {
                
                return new Promise((success,fail) =>
                {
                    success(true);
                });
            }
            else
                return new Promise((sucsess,fail) => {sucsess(false);});
        }
        catch
        {
            return new Promise((sucsess,fail) => {fail();});
        }

    }



}

export class ajaxUsers
{
    public async getAllActiveUsers() : Promise<user[]>
    {
        let server = new Server();
        let response : Response;

        try
        {
            response = await server.sendGetRequestToServer('/admin/Users/getAllActiveUsers.php')

            if(response.wereErrors == false)
            {
                // we should have recieved an array of users
                if(Array.isArray(response.data) == true)
                {
                    let arrayOfUsers : user[] = [];
                    // go through each array item and get each user
                    (response.data as Array<any>).forEach((value) =>
                    {
                        let aUser = new user();

                        aUser.id = value.id;
                        aUser.firstName = value.firstName;
                        aUser.surname = value.surname;
                        aUser.isUserActive = value.isUserActive;
                        // add user to the array
                        arrayOfUsers.push(aUser);
                    });

                    return new Promise((success,fail) =>
                    {
                        success(arrayOfUsers);
                    });
                }
                else
                {
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                }
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }


    }


    public async getAllUsers() : Promise<user[]>
    {
        let server = new Server();
        let response : Response;

        try
        {
            response = await server.sendGetRequestToServer('/admin/Users/getAllUsers.php')

            if(response.wereErrors == false)
            {
                // we should have recieved an array of users
                if(Array.isArray(response.data) == true)
                {
                    let arrayOfUsers : user[] = [];
                    // go through each array item and get each user
                    (response.data as Array<any>).forEach((value) =>
                    {
                        let aUser = new user();

                        aUser.id = value.id;
                        aUser.firstName = value.firstName;
                        aUser.surname = value.surname;
                        aUser.isUserActive = value.isUserActive;
                        // add user to the array
                        arrayOfUsers.push(aUser);
                    });

                    return new Promise((success,fail) =>
                    {
                        success(arrayOfUsers);
                    });
                }
                else
                {
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                }
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }


    }



    /**
     * Create a new user on the server (method can only be used by Admin)
     * @param firstName first name of new user
     * @param surname surname of new user
     * @param isActive if the user is currently working, set to true, else false
     * @param password password user will use to login
     * @returns the user that was added is sucsefull
     */
     public async adminCreateUser(firstName : string, surname : string, isActive : boolean, password : string): Promise<user>
     {
         let server = new Server();
         let response : Response;
 
         let jsonData = {firstName: firstName, 
                         surname: surname,
                         isActive: isActive,
                         password: password};
 
         try
         {
             response = await server.sendPostRequestToServer("/admin/Users/createUser.php",jsonData);
             
             if(response.wereErrors == false)
             {
                 let aUser = new user();
 
                 // check if we were able to add user to the register.
                 // if id was set to -1 the user was not added
                 if(response.data.id == "-1")
                    return new Promise((success, fail) =>
                    {
                        fail();
                    });
                 else
                 {// user was added to database on server
                     aUser.id = response.data.id;
                     aUser.firstName = response.data.firstName;
                     aUser.surname = response.data.surname;
                     aUser.isUserActive = response.data.isUserActive;
                 }
 
                 return new Promise((success,fail) =>
                 {
                     success(aUser);
                 });
             }
             else
             {
                 return new Promise((success, fail) =>
                 {
                     fail();
                 });
             }
         }
         catch(e)
         {
             return new Promise((success, fail) =>
             {
                 fail();
             });
         }
     }


     /**
      * 
      * @param userID the id of the user who we want to delete on the server (will also remove the user from any registers the user is in)
      * @returns true if sucsefull, else false
      */
     public async adminDeleteUser(userID : number) : Promise<boolean>
     {
        let server = new Server();
        let response : Response;

        let jsonData = {userID: userID};

        try
        {
            response = await server.sendPostRequestToServer("/admin/Users/deleteUser.php",jsonData);
            
            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    success(response.data);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((success, fail) =>
            {
                fail();
            });
        }

     }


     public async adminUpdateUser(userToUpdate : user) : Promise<user>
     {
        let server = new Server();
        let response : Response;

        let jsonData = {userId : userToUpdate.id,
                        userFirstName: userToUpdate.firstName,
                        userSurname: userToUpdate.surname,
                        usersIsActive: userToUpdate.isUserActive,
                        userNewPassword: userToUpdate.password};
        
        try
        {
            response = await server.sendPostRequestToServer("/admin/Users/updateUser.php",jsonData);
            
            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    let aUser = new user();

                    aUser.id = response.data.id;
                    aUser.firstName = response.data.firstName;
                    aUser.surname = response.data.surname;
                    aUser.isUserActive = response.data.isUserActive;

                    success(aUser);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch
        {
            return new Promise((sucsess,fail) => {fail();});
        }
     }
 




}


export class ajaxManikinLungs
{

    public async adminGetListOfManikinLungTypes() : Promise<LungType[]>
    {
       let server = new Server();
       let response : Response;

       try
       {
           response = await server.sendGetRequestToServer("/admin/ManikinLungs/GetManikinLungList.php");

           if(response.wereErrors == false)
           {
               

               let arrayOfManikinLungs : LungType[] = [];
               // go through each array item
               (response.data as Array<any>).forEach((value) =>
               {
                   let lungType = new LungType();

                   lungType.id = value.id;
                   lungType.name = value.name;


                   // add lung type to the array
                   arrayOfManikinLungs.push(lungType);
               });

                   
               return new Promise((success, fail) =>
               {

                   success(arrayOfManikinLungs);
               });

           }
           else
           {
               return new Promise((success, fail) =>
               {
                   fail();
               });
           }
       }
       catch(e)
       {
           return new Promise((sucsess,fail) => {fail();});
       }

       return new Promise((succsess,fail) => {})
    }

    public async adminGetLatestLungsGivenToActiveUsers() : Promise<LungUserGiven[]>
    {
        let server = new Server();
        let response : Response;


        try
        {
            response = await server.sendGetRequestToServer("/admin/ManikinLungs/GetLatestLungsGivenOut.php");

            if(response.wereErrors == false)
            {
                

                let arrayOfManikinLungsGivenToUsers : LungUserGiven[] = [];
                // go through each array item
                (response.data as Array<any>).forEach((value) =>
                {
                    let lungGivenToUser = new LungUserGiven();

                    lungGivenToUser.userID = value.userID;
                    lungGivenToUser.manikinLungTypeID = value.manikinLungTypeID;
                    lungGivenToUser.dateGivenManikinLung = value.dateGivenManikinLung;


                    // add lung type to the array
                    arrayOfManikinLungsGivenToUsers.push(lungGivenToUser);
                });

                    
                return new Promise((success, fail) =>
                {

                    success(arrayOfManikinLungsGivenToUsers);
                });

            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch(e)
        {
            return new Promise((sucsess,fail) => {fail();});
        }
    }


    public async adminAssignLungToUser(userID: number, manikinLungID: number, unixTimeStamp: number) : Promise<LungUserGiven>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userId : userID,
                        manikinLungID: manikinLungID,
                        unixTimeStamp: unixTimeStamp};

        try
        {
            response = await server.sendPostRequestToServer("/admin/ManikinLungs/GiveUserManikinLung.php",jsonData);

            if(response.wereErrors == false)
            {
                return new Promise((success, fail) =>
                {
                    let aLungGivenToUser = new LungUserGiven();

                    aLungGivenToUser.userID = response.data.userID;
                    aLungGivenToUser.manikinLungTypeID = response.data.manikinLungTypeID;
                    aLungGivenToUser.dateGivenManikinLung = response.data.dateGivenManikinLung;
                    

                    success(aLungGivenToUser);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch
        {
            return new Promise((sucsess,fail) => {fail();});
        }

        
    }

    public async adminGetLungsAssignedToUser(userID : number, maxNumberToReturn: number) : Promise<LungUserGiven[]>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userId : userID,
                        maxNumberToReturn: maxNumberToReturn};

        try
        {
            response = await server.sendPostRequestToServer("/admin/ManikinLungs/GetLungsAssignedToUser.php",jsonData);

            if(response.wereErrors == false)
            {
                let arrayOfManikinLungsGivenToUsers : LungUserGiven[] = [];
                // go through each array item
                (response.data as Array<any>).forEach((value) =>
                {
                    let lungGivenToUser = new LungUserGiven();

                    lungGivenToUser.userID = value.userID;
                    lungGivenToUser.manikinLungTypeID = value.manikinLungTypeID;
                    lungGivenToUser.dateGivenManikinLung = value.dateGivenManikinLung;


                    // add lung type to the array
                    arrayOfManikinLungsGivenToUsers.push(lungGivenToUser);
                });

                    
                return new Promise((success, fail) =>
                {

                    success(arrayOfManikinLungsGivenToUsers);
                });
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch
        {
            return new Promise((sucsess,fail) => {fail();});
        }
    }


    /**
     * 
     * @param userId Id of the user to look for
     * @param lungId Id of the manikin lung to look for
     * @param dateIssuedLung unix time stamp of the date the lung was issued
     * @return ture if sucsesfull, else false
     */
    public async removeLungFromUser(userId: number, lungId : number, dateIssuedLung: number) : Promise<boolean>
    {
        let server = new Server();
        let response : Response;

        let jsonData = {userId : userId,
                        lungId: lungId,
                        dateIssuedLung: dateIssuedLung};

        try
        {
            response = await server.sendPostRequestToServer("/admin/ManikinLungs/RemoveManikinLungFromUser.php",jsonData);

            if(response.wereErrors == false)
            {
                if(response.data == true)
                {
                    return new Promise((success, fail) =>
                    {

                        success(true);
                    });
                }
                else
                {
                    return new Promise((success, fail) =>
                    {

                        success(false);
                    });
                }
            }
            else
            {
                return new Promise((success, fail) =>
                {
                    fail();
                });
            }
        }
        catch
        {
            return new Promise((sucsess,fail) => {fail();});
        }
    }
}







class Server
{
    /**
     * Used to send a POST request
     * @param url The location to send the request
     * @param data the data to send
     */
    public async sendPostRequestToServer(url : string, data : string | any, contentType? : string) : Promise<Response>
    {
        let response = new Response();

        const config = {

            headers: {
          
              'Content-Type': contentType == undefined? 'application/x-www-form-urlencoded' : contentType
          
            }
            // headers: {
          
            //     'Content-Type': contentType == undefined? 'application/json' : contentType
            
            //   }
          
          }

          // data is currently set to be sent for php to read. However if we are using another backend
          // e.g. asp.net, we will need to change the way the data is sent to the server          
          switch(process.env.VUE_APP_PostType)
          {
              case 'dotnet':

                // convert the data into the corret way form data should be sent.
                // e.g. "name:bob&age:6&height:9"
                let myData : string = '';
                // go through each property in the data object we were sent
                for (const [key, value] of Object.entries(data)) 
                {
                  // convert the property name and property value to a string
                  myData += key + '=' + value + '&';
                }
                // remove the last "&" from the string and overwrite the data value passed in with
                // the new value we will be using to send to the server.
                data = myData.substring(0, myData.length - 1);
                break;
          }
          

        try
        {
            let result = await axios.post(url,data,config);
            
            //let result = await axios.post(url,JSON.stringify(data),config)

            if(result.status == 200)
            {
                response.satusCode = 200;
                response.data = result.data;
            }
            else
            {
                response.satusCode = result.status;
                response.wereErrors = true;
                response.errorData = result.data;
            }

            return new Promise((success, fail) =>
            {
                success(response);
            });
        }
        catch(e)
        {
            response.wereErrors = true;
            response.errorData = e;
            console.log(e);

            return new Promise((success, fail) =>
            {
                fail(response);
            });
        }


    }

    /**
     * Use to send a GET request
     * @param url the location to send the request
     */
    public async sendGetRequestToServer(url : string) : Promise<Response>
    {
        let response = new Response();

        try
        {
            let result = await axios.get(url);
            if(result.status == 200)
            {
                response.satusCode = 200;
                response.data = result.data;
            }
            else
            {
                response.satusCode = result.status;
                response.wereErrors = true;
                response.errorData = result.data;
            }

            return new Promise((success, fail) =>
            {
                success(response);
            });
        }
        catch(e)
        {
            response.wereErrors = true;
            response.errorData = e;

            return new Promise((success, fail) =>
            {
                fail(response);
            });
        }
    }
}

/**
 * this is the object that gets returned when calling 
 * sendPostRequestToServer, sendGetRequestToServer from the Server class
 */
class Response
{
    public data : any;
    // 200 means everything ok. 
    public satusCode : number = 0;

    public wereErrors : boolean = false;
    public errorData : any;
}

class Data
{
    data : string = '';
}