﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using EnvDTE80;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NUnit.Framework;
using SmartCmdArgs;
using SmartCmdArgsTests.Utils;
using Assert = NUnit.Framework.Assert;

namespace SmartCmdArgsTests
{
    public class SuoSavingTests : TestBase
    {
        public void SaveAndLoadSuoTest(TestLanguage language)
        {
            var package = (CmdArgsPackage)Utils.LoadPackage(new Guid(CmdArgsPackage.PackageGuidString));
            var openSolutionSuccess = OpenSolutionWithName(language, "DefaultProject");
            Assert.That(openSolutionSuccess, Is.True);
            var properties = (CmdArgsOptionPage)package.GetDialogPage(typeof(CmdArgsOptionPage));

            properties.VcsSupport = false;

            var curList = package?.ToolWindowViewModel?.CurrentArgumentList;
            Assert.IsNotNull(curList);

            var initalCommands = new[] { "arg1", "Arg2", "arg 3" };
            var initalEnabledStates = new[] { true, true, false };

            InvokeInUIThread(() =>
            {
                for (int i = 0; i < initalCommands.Length; i++)
                {
                    curList.AddNewItem(initalCommands[i], initalEnabledStates[i]);
                }
            });

            Utils.ForceSaveSolution();

            var solutionFile = Dte.Solution.FullName;
            Assert.IsTrue(File.Exists(solutionFile));

            Dte.Solution.Close();

            Assert.IsNull(package.ToolWindowViewModel.CurrentArgumentList);

            OpenSolutionWithPath(solutionFile);

            var curDataList = package?.ToolWindowViewModel?.CurrentArgumentList?.DataCollection;
            Assert.IsNotNull(curDataList);
            Assert.AreEqual(initalCommands.Length, curDataList.Count);
            for (int i = 0; i < initalCommands.Length; i++)
            {
                var curItem = curDataList[i];
                Assert.AreEqual(initalCommands[i], curItem.Command);
                Assert.AreEqual(initalEnabledStates[i], curItem.Enabled);
                Assert.AreNotEqual(Guid.Empty, curItem.Id);
            }
        }
    }
}
