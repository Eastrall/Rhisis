using Rhisis.Core.Resources;
using System.Collections.Generic;
using Xunit;

namespace Rhisis.Core.Test.Resources
{
    public class TextFileUnitTest
    {
        private static readonly string TextFilePath = "Common/text.txt.txt";

        private static readonly string TextKeyWithTabs = "IDS_TEXT_00001";
        private static readonly string TextValueWithTabs = "Hello world!";

        private static readonly string TextKeyWithSpaces = "IDS_TEXT_WITH_SPACES";
        private static readonly string TextValueWithSpaces = "This is a text is only spaces.";

        private static readonly string TextSingleCommentedKey = "IDS_TEXT_SINGLE_COMMENT";
        private static readonly string TextMultipleCommentedKey = "IDS_TEXT_COMMENTED";

        [Fact]
        public void OpenTextFile()
        {
            var textFile = new TextFile(TextFilePath);

            Assert.NotNull(textFile);
            Assert.NotEqual(0, textFile.Count);

            textFile.Dispose();
        }

        [Fact]
        public void CountTexts()
        {
            var textFile = new TextFile(TextFilePath);

            Assert.NotNull(textFile);
            Assert.NotEqual(0, textFile.Count);

            textFile.Dispose();
        }

        [Fact]
        public void GetTextValueWithTabsFromMethod()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            string text = textFile.GetText(TextKeyWithTabs);
            Assert.Equal(TextValueWithTabs, text);
            
            textFile.Dispose();
        }

        [Fact]
        public void GetTextValueWithTabsFromIndexer()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            string text = textFile[TextKeyWithTabs];
            Assert.Equal(TextValueWithTabs, text);

            textFile.Dispose();
        }

        [Fact]
        public void GetTextValueWithSpacesFromMethod()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            string text = textFile.GetText(TextKeyWithSpaces);
            Assert.Equal(TextValueWithSpaces, text);

            textFile.Dispose();
        }

        [Fact]
        public void GetTextValueWithSpacesFromIndexer()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            string text = textFile[TextKeyWithSpaces];
            Assert.Equal(TextValueWithSpaces, text);

            textFile.Dispose();
        }

        [Fact]
        public void TryGetSingleCommentedTextFromMethod()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            var exception = Assert.Throws<KeyNotFoundException>(() =>
            {
                string value = textFile.GetText(TextSingleCommentedKey);
            });

            Assert.NotNull(exception);
            Assert.Equal(typeof(KeyNotFoundException), exception.GetType());

            textFile.Dispose();
        }

        [Fact]
        public void TryGetSingleCommentedTextFromIndexer()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            var exception = Assert.Throws<KeyNotFoundException>(() =>
            {
                string value = textFile[TextSingleCommentedKey];
            });

            Assert.NotNull(exception);
            Assert.Equal(typeof(KeyNotFoundException), exception.GetType());

            textFile.Dispose();
        }

        [Fact]
        public void TryGetMultipleCommentedTextFromMethod()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            var exception = Assert.Throws<KeyNotFoundException>(() =>
            {
                string value = textFile.GetText(TextMultipleCommentedKey);
            });

            Assert.NotNull(exception);
            Assert.Equal(typeof(KeyNotFoundException), exception.GetType());

            textFile.Dispose();
        }

        [Fact]
        public void TryGetMultipleCommentedTextFromIndexer()
        {
            var textFile = new TextFile(TextFilePath);
            Assert.NotNull(textFile);

            var exception = Assert.Throws<KeyNotFoundException>(() =>
            {
                string value = textFile[TextMultipleCommentedKey];
            });

            Assert.NotNull(exception);
            Assert.Equal(typeof(KeyNotFoundException), exception.GetType());

            textFile.Dispose();
        }
    }
}
