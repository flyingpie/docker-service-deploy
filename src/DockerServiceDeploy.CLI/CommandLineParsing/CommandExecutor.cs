using CommandLine;
using CommandLine.Text;
using Flyingpie.DockerServiceDeploy.CLI.Commands;
using Serilog;
using System;
using System.Linq;
using System.Reflection;

namespace Flyingpie.DockerServiceDeploy.CLI.CommandLineParsing
{
	public static class CommandExecutor
	{
		public static int Execute(string[] args)
		{
			// Load available commands
			var commandTypes = Assembly.GetEntryAssembly().GetTypes()
				.Where(t => typeof(ICommand).IsAssignableFrom(t) || typeof(IAsyncCommand).IsAssignableFrom(t))
				.Where(t => t != typeof(ICommand) && t != typeof(IAsyncCommand))
				.ToArray();

			var parser = new Parser(config => config.HelpWriter = null);

			var exitCode = 0;

			// Parse command line options, CommandLineOptions will print a help message when the user messes this up
			var optionsParseResult = parser.ParseArguments(args, commandTypes)
				.WithParsed(command =>
				{
					try
					{
						if (command is ICommand c) c.Execute();
						else if (command is IAsyncCommand ac) ac.ExecuteAsync().GetAwaiter().GetResult();

						if (command is IDisposable disp) disp.Dispose();
					}
					catch (Exception e)
					{
						Log.Logger.Fatal(e, $"Error while executing command of type '{command.GetType()}': {e.Message}");
						exitCode = 1;
					}
				});

			if (optionsParseResult is NotParsed<object> notParsed)
			{
				if (notParsed.Errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
				{
					Console.WriteLine("ERROR(S):");
					Console.WriteLine("  No verb selected.");
					Console.WriteLine();

					PrintHelpText(args, commandTypes);
				}
				else if (notParsed.Errors.Any(e => e.Tag == ErrorType.BadVerbSelectedError))
				{
					Console.WriteLine("ERROR(S):");
					Console.WriteLine("  Verb is not recognized.");
					Console.WriteLine();

					PrintHelpText(args, commandTypes);
				}
				else
				{
					Console.WriteLine(HelpText.AutoBuild(notParsed).ToString());
				}
			}

			return exitCode;
		}

		public static void PrintHelpText(string[] args, Type[] commandTypes)
		{
			var verbs = commandTypes
				.Select(c => new
				{
					Command = c,
					Verb = c.GetCustomAttribute<VerbAttribute>(),
					VerbGroup = c.GetCustomAttribute<VerbGroupAttribute>()
				})
				.Where(v => !args.Any() || args.Any(a => v.Verb.Name.ToLower().Contains(a.ToLower())))
				.OrderBy(v => v.VerbGroup?.Name ?? string.Empty)
				.ThenBy(v => v.Verb?.Name ?? string.Empty)
				.ToList()
			;

			if (!verbs.Any()) return;

			var longestVerbName = verbs.Select(v => v.Verb.Name).OrderByDescending(n => n.Length).First();
			var padding = longestVerbName.Length + 10;

			string lastPrefix = null;
			VerbGroupAttribute lastGroup = null;

			foreach (var verb in verbs)
			{
				if (verb.Verb == null)
				{
					Console.WriteLine($"Command '{verb.Command.FullName}' is missing a verb.");
					continue;
				}

				if (verb.VerbGroup == null)
				{
					Console.WriteLine($"Command '{verb.Command.FullName}' ({verb.Verb.Name}) is missing a verb group.");
					continue;
				}

				if (verb.VerbGroup.Name != lastGroup?.Name)
				{
					if (lastGroup != null) Console.WriteLine();
					Console.WriteLine($"{verb.VerbGroup.Name}");
				}
				lastGroup = verb.VerbGroup;

				Console.WriteLine($"  {verb.Verb.Name}{new string('.', padding - verb.Verb.Name.Length)}{verb.Verb.HelpText}");
			}

			Console.WriteLine();
		}
	}
}