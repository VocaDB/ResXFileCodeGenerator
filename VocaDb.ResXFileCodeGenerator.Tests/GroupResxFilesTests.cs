using FluentAssertions;
using Xunit;

namespace VocaDb.ResXFileCodeGenerator.Tests;

public class GroupResxFilesTests
{
	[Fact]
	public void CompareWorks()
	{
		var v1 = new GroupedAdditionalFile(new AdditionalTextStub(
			@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"),
		new[]
		{
			new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"),
			new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"),
		});
		var v2 = new GroupedAdditionalFile(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"),
			new[]
			{
				new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"),
				new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"),
			}
		);
		v1.Should().Be(v2);
	}

	static readonly string[] s_data =
	{
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx",
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx",
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx",
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.da.resx",
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.resx",
		@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.vi.resx",
		@"D:\src\xhg\y\Areas\Identity\Pages\Login.da.resx", @"D:\src\xhg\y\Areas\Identity\Pages\Login.resx",
		@"D:\src\xhg\y\Areas\Identity\Pages\Login.vi.resx",
		@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.da.resx",
		@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.resx",
		@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.vi.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.cs-cz.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.da.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.de.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.es.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.fi.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.fr.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.it.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.lt.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.lv.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.nb-no.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.nl.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.nn-no.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.pl.resx", @"D:\src\xhg\y\Areas\QxModule\QtrController.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.ru.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.sv.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.tr.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.vi.resx",
		@"D:\src\xhg\y\Areas\QxModule\QtrController.zh-cn.resx",
		@"D:\src\xhg\y\DataAnnotations\DataAnnotation.da.resx",
		@"D:\src\xhg\y\DataAnnotations\DataAnnotation.resx",
		@"D:\src\xhg\y\DataAnnotations\DataAnnotation2.resx",
	};
	[Fact]
	public void FileGrouping()
	{
		var result = GroupResxFiles.Group(s_data.Select(x => new AdditionalTextStub(x)).OrderBy(x => Guid.NewGuid()).ToArray());
		var testData = new List<GroupedAdditionalFile>
		{
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.resx"), new [] {
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.da.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgControlCenter.vi.resx"),
				}
			),
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.resx"), new [] {
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.da.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\CaModule\Pages\IdfgLive.vi.resx"),
				}
			),
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\Identity\Pages\Login.resx"), new [] {
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\Identity\Pages\Login.da.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\Identity\Pages\Login.vi.resx"),
				}
			),
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.resx"), new [] {
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.da.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\Pages\QasdLogon.vi.resx"),
				}
			),
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\Areas\QxModule\QtrController.resx"), new [] {
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.cs-cz.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.da.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.de.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.es.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.fi.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.fr.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.it.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.lt.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.lv.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nb-no.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nl.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.nn-no.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.pl.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.ru.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.sv.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.tr.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.vi.resx"),
					new AdditionalTextStub(@"D:\src\xhg\y\Areas\QxModule\QtrController.zh-cn.resx"),
				}
			),
			new(new AdditionalTextStub(
				@"D:\src\xhg\y\DataAnnotations\DataAnnotation.resx"), new [] { new AdditionalTextStub(@"D:\src\xhg\y\DataAnnotations\DataAnnotation.da.resx") }
			),
			new(new AdditionalTextStub(
					@"D:\src\xhg\y\DataAnnotations\DataAnnotation2.resx"), new AdditionalTextStub[] {  }
			)
		};
		var resAsList = result.ToList();
		resAsList.Count.Should().Be(testData.Count);
		foreach (var groupedAdditionalFile in testData)
		{
			resAsList.Should().Contain(groupedAdditionalFile);
		}

	}

	[Fact]
	public void ResxGrouping()
	{
		var result = GroupResxFiles.DetectChildComboes(GroupResxFiles.Group(s_data.Select(x => new AdditionalTextStub(x)).OrderBy(x => Guid.NewGuid()).ToArray()).ToArray()).ToList();
		var expected = new List<CultureInfoCombo>
		{
			new(new []{new AdditionalTextStub("test.da.resx"), new AdditionalTextStub("test.vi.resx")}),
			new(new []{new AdditionalTextStub("test.da.resx")}),
			new(Array.Empty<AdditionalTextStub>()),
			new(new []
			{
				new AdditionalTextStub("test.cs-cz.resx"),
				new AdditionalTextStub("test.da.resx"),
				new AdditionalTextStub("test.de.resx"),
				new AdditionalTextStub("test.es.resx"),
				new AdditionalTextStub("test.fi.resx"),
				new AdditionalTextStub("test.fr.resx"),
				new AdditionalTextStub("test.it.resx"),
				new AdditionalTextStub("test.lt.resx"),
				new AdditionalTextStub("test.lv.resx"),
				new AdditionalTextStub("test.nb-no.resx"),
				new AdditionalTextStub("test.nl.resx"),
				new AdditionalTextStub("test.nn-no.resx"),
				new AdditionalTextStub("test.pl.resx"),
				new AdditionalTextStub("test.ru.resx"),
				new AdditionalTextStub("test.sv.resx"),
				new AdditionalTextStub("test.tr.resx"),
				new AdditionalTextStub("test.vi.resx"),
				new AdditionalTextStub("test.zh-cn.resx"),
			}),
		};
		result.Count.Should().Be(expected.Count);
		result.Should().BeEquivalentTo(expected);
	}
}


