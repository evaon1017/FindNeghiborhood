using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FindNeghiborhood
{
    public partial class Index : System.Web.UI.Page
    {
        private static readonly string XmlPath = HttpContext.Current.Server.MapPath("~/data.xml");

        internal static DataSet1 DS;

        static Index()
        {
            DS = new DataSet1();
            if (System.IO.File.Exists(XmlPath))
            {
                DS.Tables[0].ReadXml(XmlPath);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.UI_litLandCount.Text = DS.Tables[0].Rows.Count.ToString();

            if (this.IsPostBack == false)
            {
                if (Request.Cookies["Connected"]?.Value == "1")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "autoConnect", "setTimeout(() => $('#start-connect').click(), 100)", true);
                }
            }

            //var dt = DS.Tables[0];

            //using (var cli = new WebClient())
            //{
            //    dt.AsEnumerable().Select(r => new { Row = r, TokenId = Convert.ToInt32(r.Field<string>("TokenId")) }).Select(_item =>
            //    {
            //        var item = _item;
            //        var text = cli.DownloadString($"https://metacitym.gamamobi.com/mcm/api/land?token={item.TokenId}");
            //        var metaObject = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(text, new
            //        {
            //            attributes = new[]
            //            {
            //                    new
            //                    {
            //                        trait_type = string.Empty,
            //                        value = string.Empty
            //                    }
            //                }
            //        });

            //        var traitDict = metaObject.attributes.ToDictionary(set => set.trait_type, set => set.value);

            //        var row = dt.Rows.Find(item.TokenId);
            //        if (row == null)
            //        {
            //            row = dt.Rows.Add(item.TokenId);
            //        }

            //        row.SetField("City", traitDict.TryGetValue("city", out string city) ? city : null);
            //        row.SetField("Town", traitDict.TryGetValue("town", out string town) ? town : null);
            //        row.SetField("X", traitDict.TryGetValue("land_posx", out string land_posx) ? land_posx : null);
            //        row.SetField("Y", traitDict.TryGetValue("land_posy", out string land_posy) ? land_posy : null);
            //        return true;
            //    })
            //    .ToArray();
            //}

            //lock (Save)
            //{
            //    Save = Save.ContinueWith(t => dt.WriteXml(XmlPath));
            //}
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(this.UI_btnSend);
            ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(this.UI_btnReload);

            base.OnPreRender(e);
        }

        protected async void UI_btnSend_Click(object sender, EventArgs e)
        {
            var item = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(this.UI_hid.Value, new { address = string.Empty, token = new int[] { }, discord = string.Empty });
            var dt = DS.Tables[0];

            using (var cli = new WebClient())
            {
                foreach (var tokenId in item.token)
                {
                    var text = cli.DownloadString($"https://metacitym.gamamobi.com/mcm/api/land?token={tokenId}");
                    var metaObject = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(text, new
                    {
                        attributes = new[]
                        {
                            new
                            {
                                trait_type = string.Empty,
                                value = string.Empty
                            }
                        }
                    });

                    var traitDict = metaObject.attributes.ToDictionary(set => set.trait_type, set => set.value);

                    var row = dt.Rows.Find(tokenId);
                    if (row == null)
                    {
                        row = dt.Rows.Add(tokenId);
                    }

                    row.SetField("City", traitDict.TryGetValue("city", out string city) ? city : null);
                    row.SetField("Town", traitDict.TryGetValue("town", out string town) ? town : null);
                    row.SetField("X", traitDict.TryGetValue("land_posx", out string land_posx) ? land_posx : null);
                    row.SetField("Y", traitDict.TryGetValue("land_posy", out string land_posy) ? land_posy : null);
                    row.SetField("Discord", item.discord);

                    row["Owner"] = item.address;
                }

                lock (Save)
                {
                    Save = Save.ContinueWith(t => dt.WriteXml(XmlPath));
                }

                UI_btnReload_Click(null, null);
            }
        }

        private static Task Save = Task.CompletedTask;

        protected void UI_btnReload_Click(object sender, EventArgs e)
        {
            if (this.UI_hidAccount.Value.StartsWith("0x") == false || this.UI_hidAccount.Value.Substring(2).Any(ch => char.IsLetterOrDigit(ch) == false))
            {
                return;
            }

            var owner = this.UI_hidAccount.Value;

            this.UI_rep.DataSource =
                DS.Tables[0].Select($"Owner='{owner}'")
                .Select(row => new
                {
                    City = row.Field<string>("City"),
                    Town = row.Field<string>("Town"),
                    Cord = $"({row["X"]}, {row["Y"]})"
                })
                .GroupBy(item => new { item.City, item.Town }, item => item.Cord)
                .Select(group => new string[] { group.Key.City, group.Key.Town,
                    string.Join("<br />", group.ToArray()),
                    string.Join("<br />",
                        DS.Tables[0]
                            .Select($"City='{group.Key.City}' and Town = '{group.Key.Town}' and Owner <> '{owner}'")
                            .Select(row2 => $"({row2["X"]}, {row2["Y"]})-{row2["Discord"]}").ToArray())})
                .ToArray();

            this.UI_rep.DataBind();

            this.UP.Update();

            Response.Cookies.Add(new HttpCookie("Connected", "1") { Expires = DateTime.Now.AddDays(1) });
        }
    }
}