using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownImageFix
{
    public class MarkdownImageConverter
    {
        const string Expression = @"(!\[image.png\]\((.*?)\))";
        const string HtmlImgTagFormat = "<img src='{0}' width='{1}' />";
        const double DefaultScalingFactor = 0.56;
        private readonly Regex regex;
        private readonly HttpClient httpClient;
        private readonly double scalingFactor;

        public MarkdownImageConverter(double scalingFactor = DefaultScalingFactor)
        {
            regex = new Regex(Expression, RegexOptions.Compiled);
            httpClient = new HttpClient();
            this.scalingFactor = scalingFactor;
        }

        public string Convert(string source)
        {
            var results = regex.Matches(source);

            var builder = new StringBuilder(source);           

            var items = results.Select(async _ => (OldValue: _.Value, NewValue: await GetImageTag(_.Groups[2].Value)));

            foreach (var item in items)
            {
                builder.Replace(item.Result.OldValue, item.Result.NewValue);
            }

            return builder.ToString();
        }

        private async Task<string>GetImageTag(string imageUrl)
        {
            return string.Format(HtmlImgTagFormat, imageUrl, await GetWidth(imageUrl));
        }

        private async Task<int> GetWidth(string imageUrl)
        {
            using (var stream = await httpClient.GetStreamAsync(new Uri(imageUrl)))
            using (var image = new Bitmap(stream))
            {
                return (int) Math.Round(image.Width * scalingFactor);
            }

        }
    }
}
