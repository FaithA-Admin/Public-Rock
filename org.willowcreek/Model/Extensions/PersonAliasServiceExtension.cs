using Rock.Data;
using Rock.Model;

using System;
using System.Collections.Generic;
using System.Linq;

namespace org.willowcreek.Model.Extensions
{
    public static class PersonAliasServiceExtension
    {
        /// <summary>
        /// Attempt to delete the person and person alias
        /// </summary>
        /// <param name="personService"></param>
        /// <param name="paService"></param>
        /// <param name="personToDelete"></param>
        /// <param name="aliasToDelete"></param>
        /// <param name="errorMessaegs"></param>
        /// <returns></returns>
        public static bool DeletePersonAlias(RockContext rockContext, PersonAlias aliasToDelete, ref List<string> errorMessages)
        {
            try
            {
                //var rockContext = (RockContext)paService.Context;
                PersonAliasService paService = new PersonAliasService(rockContext);
                PersonService personService = new PersonService(rockContext);

                //Get Person Record
                //Person personToDelete = personService.Get(aliasToDelete.Person.Guid);
                Person personToDelete = aliasToDelete.Person;
                //Validate person type is reference
                if (personToDelete.ConnectionStatusValue.Value != "Reference")
                    return true;

                //Delete page views (should be only thing created once reference completes form)
                var refViews = rockContext.Interactions.Where(v => v.PersonAliasId == aliasToDelete.Id).ToList();
                rockContext.Interactions.RemoveRange(refViews);
                rockContext.SaveChanges(true);

                string errMessage;
                //Delete Alias
                if (paService.CanDelete(aliasToDelete, out errMessage))
                {
                    //NOTE - TODO - Still possibly failing to delete reference when called from referenceform code behind
                    paService.Delete(aliasToDelete);
                    rockContext.SaveChanges(true);
                }
                else if (errMessage.ToLower().Contains("page view"))
                {
                    //Delete page views again, current view doesn't get added to list until above save
                    //Tried just saving beforehand without luck
                    //TODO - Find a fix for this
                    refViews = rockContext.Interactions.Where(v => v.PersonAliasId == aliasToDelete.Id).ToList();
                    rockContext.Interactions.RemoveRange(refViews);
                    rockContext.SaveChanges(true);
                    if (paService.CanDelete(aliasToDelete, out errMessage))
                    {
                        paService.Delete(aliasToDelete);
                        rockContext.SaveChanges(true);
                    }
                    else
                        errorMessages.Add(errMessage);
                }
                else
                    errorMessages.Add(errMessage);

                //Delete Person
                if (personService.CanDelete(personToDelete, out errMessage))
                    personService.Delete(personToDelete);
                else
                    errorMessages.Add(errMessage);

                rockContext.SaveChanges(true);
                return true;
            }
            catch (Exception ex)
            {
                //Set the actual exception
                while(ex.InnerException != null)
                    ex = ex.InnerException;
                errorMessages.Add(ex.Message);
                return false;
            }
        }
    }
}
