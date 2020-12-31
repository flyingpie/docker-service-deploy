using Docker.DotNet.Models;
using Flyingpie.DockerServiceDeploy.Extensions;
using Serilog;
using System;
using System.Collections.Generic;

namespace Flyingpie.DockerServiceDeploy.Docker
{
	/// <summary>
	/// Keeps a state of a list of Docker tasks, and can detect when a list of tasks has not changed in a while.
	/// </summary>
	public class TasksListState
	{
		public static readonly TimeSpan DefaultStaleAfter = TimeSpan.FromSeconds(15);

		public TimeSpan TimeSinceLastStateChange => DateTime.UtcNow - _lastStateChange;

		public TimeSpan TimeToStale => _lastStateChange.Add(_staleAfter) - DateTime.UtcNow;

		public bool IsStale => TimeSinceLastStateChange > _staleAfter;

		private readonly TimeSpan _staleAfter;

		private string _lastState = "";
		private DateTime _lastStateChange = DateTime.UtcNow;
		private string _currentState = "";

		public TasksListState()
			: this(DefaultStaleAfter)
		{ }

		public TasksListState(TimeSpan cancelAfterTasksListStaleFor)
		{
			_staleAfter = cancelAfterTasksListStaleFor;
		}

		public void Reset() => _lastStateChange = DateTime.UtcNow;

		public TasksListState SetTasksListState(IEnumerable<TaskResponse> tasks)
		{
			_currentState = tasks.GetTasksState();

			if (_currentState != _lastState) _lastStateChange = DateTime.UtcNow;

			_lastState = _currentState;

			Log.Verbose($"Tasks list state: {_currentState}. Time since last change: {TimeSinceLastStateChange}. Is stale: {IsStale}. Time to stale: {TimeToStale}.");

			return this;
		}
	}
}