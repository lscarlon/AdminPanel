using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel
{
    //Metodi da chiamare in Program.cs - Main() per essere eseguiti all'avvio dell'applicazione
    public class StartupMethods
    {
        // Carica sul database i comandi nella relativa tabella basandosi sul customAttribute CommandName
        public void LoadCommands()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            //        var controlleractionlist = asm.GetTypes()
            //            .Where(type => typeof(Controller).IsAssignableFrom(type))
            //            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            //            .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
            //            .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name })
            //.OrderBy(x => x.Controller).ThenBy(x => x.Action).Distinct().ToList();

            IEnumerable<MemberInfo> controlleractionlist = asm.GetTypes()
               .Where(type => typeof(Controller).IsAssignableFrom(type))
               .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
               .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
               .ToList();

            Console.WriteLine("");
            Console.WriteLine("xx");
            foreach (MemberInfo a in controlleractionlist)
            {
                if (a.CustomAttributes.Count() != 0)
                {
                    foreach (CustomAttributeData attr in a.CustomAttributes)
                    {
                        if ((attr.AttributeType.Name == "CommandNameAttribute"))
                        {
                            Console.WriteLine("Controller " + a.DeclaringType.Name.Replace("Controller", ""));
                            Console.WriteLine("Action " + a.Name);
                            Console.WriteLine("CommandName " + attr.ConstructorArguments[0].Value.ToString());
                            Console.WriteLine("***************************");
                        }
                    }
                }
            };
            Console.WriteLine("xx");
            Console.WriteLine("");
        }
    }
}
