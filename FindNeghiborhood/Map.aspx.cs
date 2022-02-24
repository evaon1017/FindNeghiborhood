using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FindNeghiborhood
{
    public partial class Map : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var dt = Index.DS.Tables[0].Clone();
            dt.Merge(Index.DS.Tables[0]);

            //dt.Columns.Add("nX", typeof(int));
            //dt.Columns.Add("nY", typeof(int));
            //dt.AsEnumerable().ToList().ForEach(r =>
            //{
            //    r["nX"] = Convert.ToInt32(r.Field<string>("X"));
            //    r["nY"] = Convert.ToInt32(r.Field<string>("Y"));
            //});

            //dt.Columns.Remove("X");
            //dt.Columns.Remove("Y");
            //dt.Columns["nX"].ColumnName = "X";
            //dt.Columns["nY"].ColumnName = "Y";

            this.UI_repCities.ItemDataBound += (a, b) =>
            {
                var ph = (PlaceHolder)b.Item.FindControl("UI_ph");

                var groupedRows = (IGrouping<Tuple<string,string>, DataRow>)b.Item.DataItem;

                var minX = groupedRows.Min(r => r.Field<short>("numX"));
                var minY = groupedRows.Min(r => r.Field<short>("numY"));
                var maxX = groupedRows.Max(r => r.Field<short>("numX"));
                var maxY = groupedRows.Max(r => r.Field<short>("numY"));


                var dataSet = groupedRows.ToDictionary(r => Tuple.Create(r.Field<short>("numX"), r.Field<short>("numY")), r => new
                {
                    Token = r.Field<string>("TokenID"),
                    Cord = $"({r["X"]}, {r["Y"]})",
                    Discord = r.Field<string>("Discord"),
                });

                var mapUnitCounter = 0;
                var discordOwnerCounter = 0;

                for (short y = minY; y <= maxY; y++)
                {
                    var row = new HtmlGenericControl("tr");
                    for (short x = minX; x <= maxX; x++)
                    {
                        var cell = new HtmlGenericControl("td");
                        var key = Tuple.Create(x, y);
                        if (dataSet.ContainsKey(key))
                        {
                            var unit = dataSet[key];
                            //cell.Controls.Add(new Literal
                            //{
                            //    Text = $"ID {unit.Token}<br />{unit.Cord}<br />",
                            //});

                            HyperLink openSeaLink;
                            cell.Controls.Add(openSeaLink = new HyperLink
                            {
                                NavigateUrl = $"https://opensea.io/assets/matic/0x82016d4ad050ef4784e282b82a746d3e01df23bf/{unit.Token}",
                                Target = "_Blank",
                            });

                            //openSeaLink.Controls.Add(new Image
                            //{
                            //    Width = Unit.Pixel(20),
                            //    Height = Unit.Pixel(20),
                            //    ImageUrl = "https://seeklogo.com/images/O/opensea-logo-7DE9D85D62-seeklogo.com.png",
                            //});
                            openSeaLink.Controls.Add(new Literal
                            {
                                Text = $"ID {unit.Token}",
                            });

                            cell.Controls.Add(new Literal
                            {
                                Text = $"<br />",
                            });
                            //cell.Controls.Add(new Literal
                            //{
                            //    Text = $"ID {unit.Token}<br />{unit.Cord}<br />",
                            //});

                            HyperLink metaDataLink;
                            cell.Controls.Add(metaDataLink = new HyperLink
                            {
                                NavigateUrl = $"https://metacitym.gamamobi.com/mcm/api/land?token={unit.Token}",
                                Target = "_Blank",
                            });
                            metaDataLink.Controls.Add(new Literal
                            {
                                Text = $"{unit.Cord}",
                            });

                            //metaDataLink.Controls.Add(new Image
                            //{
                            //    Width = Unit.Pixel(20),
                            //    Height = Unit.Pixel(20),
                            //    ImageUrl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAOEAAADhCAMAAAAJbSJIAAAAk1BMVEX///9iaI+KkrJFSnV5f6JjaZBESHRXXISJkbFgZo6EjK5YX4ldY4yNlbReZIxla5Hm5+3u7/M3PW3a3OZpb5Ts7fL5+fvh4umxtMY+Q3HFydjO0d6jqcKLj6ubor18gaGrrsG7v9Gyt8ymrMSbn7bIy9pyeJpQVX6HjKgzOWulqL1jZ4lRWIWxs8XS1N6eobh7fpo2VTnNAAAJ9klEQVR4nO2dZ3viOhCFVxTHcgEMxMRpQAqELEvu//911zbFbdRM8Eh+fD7vJnoj+cyojPTnT6dOnTp16tSp069rgt2AWyuYB9hNuLHeNy/YTbitZs7APWA34pYK/d5gtMFuxS21dvwBdR+wm3E7BU4vJiRee83m2T4SttZsZk4vJSStNRvfPhHSOXZTbqN13IVHQuL+w27MLXRv2xdCQu6xm3MDvSeAZ8I2mk1iM7k+bKHZ+HaBsH1msz524YWQuCvsJv2ughNgRkjcdpnNs10hbJfZHM5dmCMk7hN2s35PoW8DhHQTYjfs17S+dGGesEWZzX0GWCCkrTGbVxsmJKMldtN+R4dcFxYJW2I2OZupENJ5G8xmne/CEiHxVtjNu16TAmCZkIzMXwN/trmE3hd2A6/VodiFFULifmI38TqFPVtASMfYbbxO61IXVgmJ+43dyGs0KQMChMQ12WxebQnCkcFm81TpQoiQuI/YDa2rYjbDJjQ3s6nYDIPQ2DWbqs2wCE01m6rNMAnNnEYBNsMkJHsDM5tKNsMlpGPzzOYN7EIWIfGMy2wWMCCTkLoL7CYrCrQZDqFxmQ1sMzxCwzKb0GZ0IYeQUJPMhmEzfELvB7vZ8gKzGSEhMchsWDYjIPSMyWyYNiMgNMZsGNmMBKEpmQ3bZkSEhpgNx2aEhGaYDTCxlyekBhw+5dmMmNAAs2FnM3KE+mc2XJuRIXQ/sBH4Yk2aFPrQnWFDcMXJZiQJNc9sPkVdKEFI3DU2BluhEFCGkLj6ms27aIzKEXramo3QZiQJibZmI7QZWUKqqdk8SnShHKGmZhMIshkVQjLUsaxmJdOFsoQ6mo2MzcgT6mg2/EmTMqF+J92lbEaBkOhWwydnMyqEhOplNitJQAVCvcxmJjlGVQj1MhtJm1Ej1GnNRtZm1Ag1ymwCeUAlQuLqYjYSk6Z6hCNNymrkbSYlHA4VOlGPGj4Fm+kt53fb0ViaUQ+zkbYZvzcY0+GdZfXvpBl1yGxksxnfHgwpJTFhP2EcjiV7Eb+sRs5mfHtDUotJCGNZ/a0cI34Nn5TN+M58dGrxiTCB3BKZwbrHNhuxzfj+8sKXJ0wYR+KOxJ5GQUdIi3yJveQanCfsS5kObg2fyGZO9sIilGMcYZoN32Zie6HlDKZEmEpgOphmw7MZP2cvAsL4g6S8jsTLbKCT6qC9iAhT02Ez4hUMM22mbC9iwiRCsj9IrMyGNWmK+Ujl+xMSHk0H/iAp0jQK3qaI7WXEmSCxCVPILQGnHjiHT8t1dyd72YCfnxwh23QwCoYBm0nsRTS/FRHGgkwHw2yqdXc+017UCEHTaT6zuS8Bcu1FkfA4vSoxek2X1RTLewX2okqYQt4VTafpzCZvM4m9yHSfEmE6hSyYTsNmkx0hlbGXWoSJ8qZDh00CXmxGzl5qExZMp8kavvMR0nhyJD086xGmjKfvkTZYw3e0GZXPrz5hynhc72gus0lsJpkcqfPVIsxMp6nbCeJsRtFeriXsn6ZXTRUMJxfJDuvx1SdMOtIbN3M7wcQZkLp81xCmH+S+CbN5dL3afNcR9vvRtJEtxcmLi9SHu/+aiheHsYtAOO03uSL1b1RzqNb20ihquFI4+Kg3VGsSWtOv5teFZ4M6Q7Ue4e4vzpLp2lMfqnWytihC2ycN1YeqOmE0/cI82b5YKjKqElrTv9jlbI9zpc9RjdCabjU4NRR+u/wl0vqE0e5DjzNDk+VeeqgqEFo7pAEKxaVP6aEqTzjdQqWITaRt4SvwW8KVZOSQJYyiH8BBZ8tGRu3CeQN+uWQ+LkcY59jAAA0+mjqYsXZ60NrlYS7BKEU43UI/f033qxuTXfRqO6+QCTyIp45iQiuaQiCzjdvgbVKBbdvOCvgkAuFQFRLGKQz4g2O7bnLrIinYdmzI6w6CfFxECObY4UPiY/tGy7zfY8R4qEJnzdfcqSOfMLKgTbSnTfJXa3hr5nj3he08QyPqg5MA8AitHThAv9KRT0nD+fepzMl21sAvXgyYnyPnpAKYY4er/TEnbP5QzXlzxvHBocqKHExCOMe+JEsY97iej2LEQxXwuOAbjhwMwmgK5diLr/PfiQ5uD1RRts9t29BQhfNx+NTXbg4N0G/vPGlpctspp1zxPZzkfI6r3QgRwjn2mmYDHau25DlDjCOHXD5eJYxzbOBnL5a5uIp26XfhSI3tvAFf0uSr5DhlQmsKrWMX1yrpGG0eXDx96fSg7b3DxmUTWtM7MMceFv4P5gsKxbtaWPn43oMJrWi3Av79rPg3IS7q5VGl03u28y7Ix3OE8QBl5dh5IdfNlI9GxYz8fPxCGKcwYI5dCaPYdwxXa4IY+Tj1CoSRBa1jP20qMxP8uqDnyhFFxlA92uOR0Nq9AP/kvmy8mIEiUwDcsBfn48C/XGzcU90TvI69AtZdKdFgwRQ8sM/Ix8deTBhtoYH3OYZydT2e2gGv94rzcWBhNfjxtoIcuzhGNalYh4sS4Hx88QLl2D8euDGAXfN0EeumRMeXG2NxCgNPJvfY204XscpI4Xy8pEU1QmgTKDJVQ8aFEVofz4lzHkCDQJEJChnnoQrm42c9EPbC3BC/QDYnzo2XjHw80YE5QIl+t2C+c8rY4iQHGqr8BXL8AuCyeJcHQ/l4+I+7yaHhM1CCe6LK+fjThr/DodXVLScJSoIL+XhlcaMCuMIDYUt05V6WjwvPNmj6Ksu98HYFp5eMvUcq3ElFLW7mSHBJctqNz7Ol+FCDboEiEy9knLURn2jQL1BcJLoIuid1A0/j+2gqEl8tKHPLroaBIpP4Fgkhoe7vIYpChpBQ00CRqbKAqkrYeK2osgQXXgvv1dfgtKVIzNmwDKERrz5xbssQEhrySAn3YiU+oSb3swnFex+B/5KO5oEiU83XkPS4f05K9V60QjpwUU/se/g4hCYEikzMkMEm1Gp5VCzmJWBMQqrZzchCsUIGk1CPfTQVqb5DasRrXQUxUhvWW7Jo13ldIThksN4D1mYfTUXgbJjxarVG+2gqglIbkNCwQJEJ2nMDX4/Xax9NRcACKkSo7/KoWNUFVIBQlwMX9VQJGVVCbQ5c1FNlAbVKqPfyqFjlkFEhxL1E9zdUChllQu2XR8Uq7bmVCTW4kfxqFUNGidDkQJGpEDKKhBrvo6mosOdWIKTaP6sqqXzIKBDuDQ8UmXIhI0+o54GLespCRo6wBYEiUxYy8oQGLY+KddlzywjNWh4V67yAeiE0dtbLUniaDZ8J6bAlgSLTaQH1TGjKPpqKjguoJ0Jz9tEUFKYh40g4MmcfTUXpAuqRUP8DF/WU7LmlhG0LFJnicZoQti5QZApsOyE0bR9NRTMnJmxjoMj05gxwC5dvrvB1Y+I+moomQyP30VTUesBOnTp16tSpE4b+B33jyP5yfvN7AAAAAElFTkSuQmCC",
                            //});

                            cell.Controls.Add(new Literal
                            {
                                Text = $"<br />",
                            });

                            if (string.IsNullOrWhiteSpace(unit.Discord) == false)
                            {
                                cell.Attributes["class"] = $"hasOwner";
                                cell.Controls.Add(new Literal
                                {
                                    Text = $"<br />",
                                });
                                cell.Controls.Add(new HyperLink
                                {
                                    NavigateUrl = "#",
                                    Text = $"discord",
                                    ToolTip = unit.Discord,
                                });
                                discordOwnerCounter++;
                            }

                            mapUnitCounter++;
                        }

                        row.Controls.Add(cell);
                    }
                    ph.Controls.Add(row);
                }

                ((HtmlGenericControl)b.Item.FindControl("UI_mapName")).InnerText = $"{groupedRows.Key.Item1} - {groupedRows.Key.Item2} ※ {discordOwnerCounter} Owned in {mapUnitCounter} Cell";
            };
            this.UI_repCities.DataSource = dt.AsEnumerable().GroupBy(r => Tuple.Create(r.Field<string>("City"), r.Field<string>("Town"))).OrderBy(gr => gr.Key).ToArray().ToList();
            this.UI_repCities.DataBind();
        }
    }
}