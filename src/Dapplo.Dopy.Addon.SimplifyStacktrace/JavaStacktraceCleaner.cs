//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Dopy
// 
//  Dapplo.Dopy is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Dopy is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Dopy. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dapplo.Dopy.Addon.SimplifyStacktrace
{
    /// <summary>
    /// A utility class to cleanup a Java stacktrace
    /// </summary>
    public class JavaStacktraceCleaner
    {
        private readonly IList<string> _ignorables = new List<string>
        {
            @".*\.invoke$",
            @"^org\.apache\..*",
            @"^org\.hibernate\..*",
            @"^sun\.reflect\..*",
            @"^javax\.servlet\..*",
            @"^org\.springframework\..*",
            @"^org\.apache\.wicket.*",
            @"^org\.jboss\..*",
            @"^net\.sf\..*",
            @"^com\.ibm\..*",
            @"^oracle\..*",
            @"^.*\$Proxy.*",
            @"^java\.lang\.reflect\..*",
            @"^java\.security\..*",
            @"^javax\.security\..*",
            @"^java\.lang\.Thread\..*",
            @"^org\.eclipse\.jetty.*",
            @"^java\.io\..*",
            @"^java\.util\..*"
        };
        private const string RegexpExceptionClassname = @"[a-zA-Z_\$0-9]+(?:\.[a-zA-Z_\$0-9]+)+\((?:[a-zA-Z_\$0-9\.]+:[0-9]+|Unknown Source)\)";

        /// <summary>
        /// Create a stacktrace cleaner
        /// </summary>
        /// <param name="stacktrace"></param>
        public JavaStacktraceCleaner(string stacktrace)
        {
            if (string.IsNullOrEmpty(stacktrace))
            {
                return;
            }

            if (!stacktrace.Contains("Exception"))
            {
                return;
            }
            IsStacktrace = true;

            string[] lines = stacktrace.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var newText = new StringBuilder();

            var preprocessedLines = lines.SelectMany(line =>
            {
                if (!Regex.IsMatch(line, RegexpExceptionClassname))
                {
                    return Enumerable.Repeat(line, 1);
                }
                var ats = Regex.Split(line, $@"\s+at\s+({RegexpExceptionClassname})");

                if (ats.Length <= 1)
                {
                    return Enumerable.Repeat(line, 1);
                }
                return ats.Where(atLine => !string.IsNullOrWhiteSpace(atLine)).Select(atLine => Regex.IsMatch(atLine, RegexpExceptionClassname) ? $"\tat {atLine}" : atLine);
            });

            // Make a linq query which skips all the ignored lines
            var stacktraceLines = preprocessedLines
                .Where(line =>
                    {
                        if (!line.StartsWith("\tat"))
                        {
                            return true;
                        }
                        var matches = Regex.Match(line, RegexpExceptionClassname);
                        return !matches.Success || !_ignorables.Any(ignorable => Regex.IsMatch(matches.Value, ignorable));
                    }
                );
            int newLineCount = 0;
            foreach (var stacktraceLine in stacktraceLines)
            {
                newLineCount++;
                newText.AppendLine(stacktraceLine);
            }

            // If the new line count is smaller than the original, we have modifications.
            HasModifications = newLineCount < lines.Length;
            CleanStacktrace = newText.ToString();
        }

        /// <summary>
        /// Were modifications made
        /// </summary>
        public bool HasModifications { get; }

        /// <summary>
        /// Test if the supplied string is a stacktrace
        /// </summary>
        public bool IsStacktrace { get; }
        
        /// <summary>
        /// The cleaned stacktrace
        /// </summary>
        public string CleanStacktrace { get; }
    }
}
