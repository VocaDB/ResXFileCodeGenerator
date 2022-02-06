using System.Globalization;
using FluentAssertions;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests.IntegrationTests;

public class TestResxFiles
{
	[Fact]
	public void TestNormalResourceGen()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da");
		Test1.CreateDate.Should().Be("OldestDa");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ch");
		Test1.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test1.CreateDate.Should().Be("OldestEnUs");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
		Test1.CreateDate.Should().Be("OldestDaDK");
	}
	[Fact]
	public void TestCodeGenResourceGen()
	{
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da");
		Test2.CreateDate.Should().Be("OldestDa");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Test2.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("ch");
		Test2.CreateDate.Should().Be("Oldest");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		Test2.CreateDate.Should().Be("OldestEnUs");
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("da-DK");
		Test2.CreateDate.Should().Be("OldestDaDK");
	}

}
