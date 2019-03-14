using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;

namespace AdminPanel
{
    //Metodi da chiamare in Program.cs - Main() per essere eseguiti all'avvio dell'applicazione
    public static class StartupMethods
    {
        public static void RunAll()
        {
            LoadCommands();
            ClearClaims();
        }
        
        // Carica sul database i comandi nella relativa tabella basandosi sul customAttribute CommandName
        public static void LoadCommands()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            AppDbContext db = Database.dbContext;
            IEnumerable<Command> AppCommandsList =  asm.GetTypes()                                                              //   Lista delle action con CommandName
                .Where(type => typeof(Controller).IsAssignableFrom(type))                                                       //seleziono i controller
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))   //seleziono i loro metodi (Action)
                .Where(m => m.CustomAttributes.Any(ca => ca.AttributeType.Name== "CommandAuthorizeAttribute"))                       //prendo solo quelli che hanno l'attributo che mi interessa
                .Select(c => new Command {                                                                                      //creo una lista di elementi di tipo Command 
                        Controller = c.DeclaringType.Name.Replace("Controller", ""),
                        Action = ActionName(c),
                        CommandName = c.CustomAttributes.First(ca => ca.AttributeType.Name== "CommandAuthorizeAttribute").ConstructorArguments[0].Value.ToString()
                    })
                .Distinct();

            IEnumerable<Command> DBCommandsList = db.Commands;                                                                  //    Lista delle action da DB

            //Rimuovo dal db le action che non ci sono più
            db.Commands.RemoveRange(
                DBCommandsList.Where(dbC => !AppCommandsList.Any(AppC =>
                                                        AppC.Controller == dbC.Controller &&
                                                        AppC.Action == dbC.Action &&
                                                        AppC.CommandName == dbC.CommandName))
            );
            db.SaveChanges();

            DBCommandsList = db.Commands;

            //aggiungo al db le action con CommandName che non ci sono già
            db.Commands.AddRange(
                AppCommandsList.Where(AppC => !DBCommandsList.Any(dbC => 
                                                        dbC.Controller == AppC.Controller &&
                                                        dbC.Action == AppC.Action &&
                                                        dbC.CommandName == AppC.CommandName))
            );
            db.SaveChanges();
            
            //Console.WriteLine("");

        }

        // Pulisce le tabelle AspNetRoleClaims e AspNetUserClaims eliminando i record con comandi non esistenti. Per i ruoli c'è la FK
        public static void ClearClaims()
        {
            AppDbContext db = Database.dbContext;
            db.RoleClaims.RemoveRange(
                db.RoleClaims.Where(rc => 
                                         rc.ClaimType == "CommandAuthorize" 
                                         && !db.Commands.Any(c => c.CommandName == rc.ClaimValue))
            );
            db.SaveChanges();
        }
        

        // ritorna il nome della Action tenendo conto dell'attributo ActionName se presente
        private static string ActionName(MethodInfo action)
        {
            var ActionNameAttribute = action.CustomAttributes.FirstOrDefault(ca => ca.AttributeType.Name == "ActionNameAttribute");
            if (ActionNameAttribute is null) {
                return action.Name;
            } else {
                return ActionNameAttribute.ConstructorArguments[0].Value.ToString();
            }
        }
    }
}
