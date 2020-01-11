using System.Data.Common;
using System.Configuration;
using static System.Console;

/*
 * Author: Noah Gumm
 * Date: 10/19
 * 
 * Shows the functionality of a simple data provider.
 * Connects to a database (originally one with an inventory of cars)
 * Then proceeds to print create a command to get all cars from the invenory
 * And display them to the user
*/

namespace DataProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get connection string/provider from *.config
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.AppSettings["connectionString"];

            //Get factory provider
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);

            //Get connection object
            using (DbConnection connection = factory.CreateConnection()) {
                //Make sure connection isn't null
                if (connection == null) {
                    ShowError("Connection");
                    return;
                }

                //Let user see connection type
                WriteLine($"Your connection object is a : {connection.GetType().Name}");
               
                connection.ConnectionString = connectionString; //open the connection based on path from App.config
                connection.Open();
            
                DbCommand command = factory.CreateCommand(); //Create command object

                //Make sure the command object was found and exists
                if (command == null) {
                    ShowError("Command");
                    return;
                }

                //Let user see command object type
                WriteLine($"Your command object is a: {command.GetType().Name}");
                
                command.Connection = connection; //Set connection for command              
                command.CommandText = "Select * FROM Inventory"; //Set command text to select all data from inventory table

                //Print out data using data reader
                using (DbDataReader dataReader = command.ExecuteReader()) {

                    WriteLine($"Your data reader object is a: {dataReader.GetType().Name}");
                    WriteLine("\n***** Current Inventory *****");

                    //Print out all car objects from inventory
                    while (dataReader.Read()) {
                        WriteLine($"-> Car #{dataReader["carID"]} is a {dataReader["Make"]}.");
                    }
                }
                ReadLine();
            }
        }
        //Custom error function for if connection or command object fails to initilize
        private static void ShowError(string objectName)
        {
            WriteLine($"There was an issue creating the {objectName}");
            ReadLine();
        }
    }
}
