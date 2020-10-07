/*
 * MIT License
 *
 * Copyright (c) 2019-2020 Kim J. Nordmo and Contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Cake.Warp
{
    using System;
    using System.Collections.Generic;
    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Tooling;

    /// <summary>
    /// The runner implementation responsible for
    /// passing the correct arguments to the warp packer.
    /// </summary>
    internal sealed class WarpRunner : Tool<WarpSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarpRunner" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        internal WarpRunner(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
        }

        /// <summary>
        /// Runs the tool using the specified settings.
        /// </summary>
        /// <param name="settings">The settings to run the tool with.</param>
        internal void Run(WarpSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            ValidateSettingsProperties(settings);
            this.Run(settings, GetArguments(settings));
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            yield return "warp-packer";
            yield return "warp-packer.exe";
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return "Warp Packer";
        }

        private static ProcessArgumentBuilder GetArguments(WarpSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            // An enum should always have a value.
            string architecture;
            switch (settings.Architecture)
            {
                case WarpPlatforms.LinuxX64:
                    architecture = "linux-x64";
                    break;
                case WarpPlatforms.MacOSX64:
                    // ReSharper disable once StringLiteralTypo
                    architecture = "macos-x64";
                    break;
                case WarpPlatforms.WindowsX64:
                    architecture = "windows-x64";
                    break;
                default:
                    throw new NotSupportedException("The specified platform/architecture is not supported");
            }

            builder.AppendSwitch("--arch", architecture);

            builder.AppendSwitchQuoted("--input_dir", settings.InputDirectory.FullPath);

            builder.AppendSwitchQuoted("--exec", settings.ExecutableName);

            builder.AppendSwitchQuoted("--output", settings.OutputFilePath.FullPath);

            return builder;
        }

        private static void ValidateSettingsProperties(WarpSettings settings)
        {
            if (settings.InputDirectory == null)
            {
                throw new ArgumentNullException(nameof(settings.InputDirectory));
            }

            if (string.IsNullOrWhiteSpace(settings.ExecutableName))
            {
                throw new ArgumentNullException(nameof(settings.ExecutableName));
            }

            if (settings.OutputFilePath == null)
            {
                throw new ArgumentNullException(nameof(settings.OutputFilePath));
            }

            // We could handle whether the directory/files exist,
            // we will let warp packer handle this for now.
        }
    }
}
