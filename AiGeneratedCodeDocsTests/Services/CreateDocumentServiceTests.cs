using Microsoft.VisualStudio.TestTools.UnitTesting;
using AiGeneratedCodeDocs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Services.Tests;

[TestClass()]
public class CreateDocumentServiceTests
{
    [TestMethod]
    public void Test_SplitCodeToSections_EmptyCode()
    {
        string code = string.Empty;
        var result = CreateDocumentService.SplitCodeToSections(code);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Test_SplitCodeToSections_NullCode()
    {
        string code = null;
        var result = CreateDocumentService.SplitCodeToSections(code);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Test_SplitCodeToSections_OneSectionCode()
    {
        string code = "public int numOfTests = 5;";
        var result = CreateDocumentService.SplitCodeToSections(code);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("public int numOfTests = 5;\n", result[0]);
    }

    [TestMethod]
    public void Test_SplitCodeToSections_MultipleSectionsCode()
    {
        string code = "public int numOfTests = 5;\nprivate string testName = \"Test\";";
        var result = CreateDocumentService.SplitCodeToSections(code, 5);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("public int numOfTests = 5;\n", result[0]);
        Assert.AreEqual("private string testName = \"Test\";\n", result[1]);
    }
}