using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace WindowsFormsApp1
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Profile profile;
        public Profile profile2;
        public Match match;
        public bool isLogin1Loaded = false;
        public bool isLogin2Loaded = false;
        public bool isMatchLoaded = false;
        public bool showDEmaps = true;
        public bool showAIMmaps = true;
        public bool loadAvatars = true;

        private static string client_key_api = "Bearer 3a043af9-c274-4842-8985-83fdb68b7e1c";

        public static JObject GetAPI(string nickname)
        {
            if(ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/players?nickname=" + nickname + "&game=csgo");
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    WebResponse res = req.GetResponse();
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return JObject.Parse(sr.ReadToEnd());
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Profile or CS: GO stats not found.";
                    form.ShowDialog();
                    return new JObject();
                }
            }
            else if(ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.LimitedAccess)
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return new JObject();
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return new JObject();
            }
        }
        public static JObject GetProfile(string guid, bool csgo)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/players/" + guid + (csgo ? "/stats/csgo" : ""));
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    using (WebResponse res = req.GetResponse())
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return JObject.Parse(sr.ReadToEnd());
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    JObject json = (JObject)ex.Response.ToString();
                    form.errorMsg.Text = "Profile or CS: GO not found.";
                    form.ShowDialog();
                    return new JObject();
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return null;
            }
            
        }
        public static string GetProfile(string guid)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/players/" + guid + "/stats/csgo");
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    using (WebResponse res = req.GetResponse())
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Profile or CS: GO stats not found.";
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return null;
            }
        }
        public static JObject GetStats(string playerID)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/players/" + playerID + "/stats/csgo");
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    using (WebResponse res = req.GetResponse())
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return JObject.Parse(sr.ReadToEnd());
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Account not have csgo stats.";
                    form.Show();
                    return new JObject();
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return new JObject();
            }
            
        }
        public static JObject GetMatch(string MatchID)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/matches/" + MatchID);
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    using (WebResponse res = req.GetResponse())
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return JObject.Parse(sr.ReadToEnd());
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Bad Match ID. Match not found.";
                    form.ShowDialog();
                    return new JObject();
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                form.Show();
                return new JObject();
            }
            
        }

        public static string GetRanking(string region, string guid)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                WebRequest req = WebRequest.Create("https://open.faceit.com/data/v4/rankings/games/csgo/regions/" + region.ToUpper() + "/players/" + guid);
                req.Method = "GET";
                req.Timeout = 10000;
                req.Headers.Add("Authorization", client_key_api);
                req.ContentType = "application / json";
                try
                {
                    using (WebResponse res = req.GetResponse())
                    using (Stream receiveStream = res.GetResponseStream())
                    using (StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (WebException ex) when (ex.Response != null) // Раскоментировать в C#
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Ranking not loaded. Try later...";
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                return null;
            }
        }

        public static Image GetAvatar(string avatarURL)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                try
                {
                    if (avatarURL == null || avatarURL == "")
                    {
                        WebRequest req = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/avatars/avatar_default_user_300x300.jpg");
                        WebResponse res = req.GetResponse();
                        return Image.FromStream(res.GetResponseStream());
                    }
                    else
                    {
                        WebRequest req = WebRequest.Create(avatarURL);
                        WebResponse res = req.GetResponse();
                        return Image.FromStream(res.GetResponseStream());
                    }
                }
                catch (WebException ex) when (ex.Response != null)
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Avatar not loaded. Try later...";
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                return Image.FromFile(Application.StartupPath + @"\level\10.png");
            }
        }

        public static Uri SetUri(string URL)
        {
            return new Uri(URL.Replace("{lang}", "en"));
        }

        public static Image GetImageLevel(int level)
        {
            Image img = Image.FromFile(Application.StartupPath + @"\level\" + level + ".png");
            return img;
        }

        public Form1()
        {
            InitializeComponent();
            Main();
        }

        private void Main()
        {

            metroPanel1.Visible = false;
            metroPanel3.Visible = false;
            metroPanel4.Visible = false;
            metroTabControl1.SelectedIndex = 0;
        }
        
        #region Login1
        // Очистить табло логин1
        public void ClearProfileTab()
        {
            friendsBoxProfile1.Items.Clear();
            map1BoxProfile1.Items.Clear();
            map2BoxProfile1.Items.Clear();
        }
        // Показать информацию о map1
        private void map1BoxProfile1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMapBox();
            loginMapBox1Profile1_defaultStyle();
        }
        // Показать информацию о map2
        private void map2BoxProfile1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMapBox2();
            loginMapBox2Profile1_defaultStyle();
        }
        // Показать информацию профиля
        public void ShowInfoProfileTab(Profile acc)
        {
            if (isLogin1Loaded)
            {
                for (int i = 0; i < profile.friends.Count; i++)
                {
                    if(profile.friends[i] != null)
                    {
                        friendsBoxProfile1.Items.Add(profile.friends[i]);
                    }
                }

                avatarProfile1.Image = profile.imgAvatar;
                levelProfile1.Text = profile.faceit_level.ToString();
                eloProfile1.Text = profile.faceit_elo.ToString();
                nicknameProfile1.Text = profile.nickname;
                countryProfile1.Text = profile.country;
                membershipProfile1.Text = profile.membership_type;
                steamid64Profile1.Text = profile.steamid64;

                avgHSProfile1.Text = profile.avgHS.ToString() + "%";
                avgKDProfile1.Text = profile.avgKD.ToString();
                curWinStrikeProfile1.Text = profile.currrentWinStreak.ToString();
                longestWinStrikeProfile1.Text = profile.longestWinStreak.ToString();
                matchesProfile1.Text = profile.matches.ToString();
                winMatchesProfile1.Text = profile.winMatches.ToString();
                winRateProfile1.Text = profile.winRate.ToString() + "%";
                LoadMapInComboBox(map1BoxProfile1, profile.maps);
                LoadMapInComboBox(map2BoxProfile1, profile.maps);
                map1BoxProfile1.SelectedIndex = 0;
                map2BoxProfile1.SelectedIndex = 0;
                metroPanel1.Visible = true;
            }
        }
        // Обновить информацию о map1
        public void UpdateMapBox()
        {
            int index = map1BoxProfile1.SelectedIndex;
            modeProfile1Map1.Text = profile.maps[index].mode;
            matchesProfile1Map1.Text = profile.maps[index].matches.ToString();
            winsProfile1Map1.Text = profile.maps[index].wins.ToString();
            winRateProfile1Map1.Text = profile.maps[index].winRate.ToString() + "%";
            roundsProfile1Map1.Text = profile.maps[index].rounds.ToString();
            killsProfile1Map1.Text = profile.maps[index].kills.ToString();
            deathProfile1Map1.Text = profile.maps[index].death.ToString();
            kdRatioProfile1Map1.Text = profile.maps[index].kdratio.ToString();
            krRatioProfile1Map1.Text = profile.maps[index].krratio.ToString();
            mvpsProfile1Map1.Text = profile.maps[index].mvps.ToString();
            totalHSProfile1Map1.Text = profile.maps[index].totalHeadshots.ToString();
            assistsProfile1Map1.Text = profile.maps[index].assist.ToString();
            avgKDRatioProfile1Map1.Text = profile.maps[index].avgKDr.ToString();
            avgKRRatioProfile1Map1.Text = profile.maps[index].avgKRr.ToString();
            pentaKillsProfile1Map1.Text = profile.maps[index].pentaKills.ToString();
            avgPentaKillsProfile1Map1.Text = profile.maps[index].avgPentaK.ToString();
            quadroKillsProfile1Map1.Text = profile.maps[index].quadroKills.ToString();
            avgQuadroKillsProfile1Map1.Text = profile.maps[index].avgQuadroK.ToString();
            tripleKillsProfile1Map1.Text = profile.maps[index].tripleKills.ToString();
            avgTripleKillsProfile1Map1.Text = profile.maps[index].avgTripleK.ToString();
            hsPerMatchProfile1Map1.Text = profile.maps[index].hsPerMatch.ToString();
            avgKillsProfile1Map1.Text = profile.maps[index].avgKills.ToString();
            avgDeathProfile1Map1.Text = profile.maps[index].avgDeath.ToString();
            avgMVPsProfile1Map1.Text = profile.maps[index].avgMVPs.ToString();
            avgAssistsProfile1Map1.Text = profile.maps[index].avgAssist.ToString();
            avgHSsProfile1Map1.Text = profile.maps[index].avgHS.ToString();
        }
        // Обновить информацию о map2
        public void UpdateMapBox2()
        {
            int index = map2BoxProfile1.SelectedIndex;
            modeProfile1Map2.Text = profile.maps[index].mode;
            matchesProfile1Map2.Text = profile.maps[index].matches.ToString();
            winsProfile1Map2.Text = profile.maps[index].wins.ToString();
            winRateProfile1Map2.Text = profile.maps[index].winRate.ToString() + "%";
            roundsProfile1Map2.Text = profile.maps[index].rounds.ToString();
            killsProfile1Map2.Text = profile.maps[index].kills.ToString();
            deathProfile1Map2.Text = profile.maps[index].death.ToString();
            kdRatioProfile1Map2.Text = profile.maps[index].kdratio.ToString();
            krRatioProfile1Map2.Text = profile.maps[index].krratio.ToString();
            mvpsProfile1Map2.Text = profile.maps[index].mvps.ToString();
            totalHSProfile1Map2.Text = profile.maps[index].totalHeadshots.ToString();
            assistsProfile1Map2.Text = profile.maps[index].assist.ToString();
            avgKDRatioProfile1Map2.Text = profile.maps[index].avgKDr.ToString();
            avgKRRatioProfile1Map2.Text = profile.maps[index].avgKRr.ToString();
            pentaKillsProfile1Map2.Text = profile.maps[index].pentaKills.ToString();
            avgPentaKillsProfile1Map2.Text = profile.maps[index].avgPentaK.ToString();
            quadroKillsProfile1Map2.Text = profile.maps[index].quadroKills.ToString();
            avgQuadroKillsProfile1Map2.Text = profile.maps[index].avgQuadroK.ToString();
            tripleKillsProfile1Map2.Text = profile.maps[index].tripleKills.ToString();
            avgTripleKillsProfile1Map2.Text = profile.maps[index].avgTripleK.ToString();
            hsPerMatchProfile1Map2.Text = profile.maps[index].hsPerMatch.ToString();
            avgKillsProfile1Map2.Text = profile.maps[index].avgKills.ToString();
            avgDeathProfile1Map2.Text = profile.maps[index].avgDeath.ToString();
            avgMVPsProfile1Map2.Text = profile.maps[index].avgMVPs.ToString();
            avgAssistsProfile1Map2.Text = profile.maps[index].avgAssist.ToString();
            avgHSsProfile1Map2.Text = profile.maps[index].avgHS.ToString();
        }

        private void compareMaps1_Click(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile1_defaultStyle();
            loginMapBox1Profile1_defaultStyle();
            loginMapBox2Profile1_defaultStyle();
            CompareMap1andMap2Profile1_Style();
            SelectBestMapProfile1();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + profile.steamid64);
        }

        private void goToFaceit1_Click(object sender, EventArgs e)
        {
            Process.Start(profile.faceit_url);
        }

        #endregion
        public void LoadMapInComboBox(ComboBox box, List<Map> maps)
        {
            for (int i = 0; i < maps.Count; i++)
            {

                if (maps[i].mapName.StartsWith("aim_"))
                {
                    box.Items.Add(maps[i].mapName + " " + maps[i].mode);
                }
                if (maps[i].mapName == "de_cache")
                {
                    
                    box.Items.Add("Cache " + maps[i].mode);
                }
                if (maps[i].mapName == "de_cbble")
                {
                    box.Items.Add("Cobblestone " + maps[i].mode);
                }
                if (maps[i].mapName == "de_dust2")
                {
                    box.Items.Add("Dust2 " + maps[i].mode);
                }
                if (maps[i].mapName == "de_mirage")
                {
                    box.Items.Add("Mirage " + maps[i].mode);
                }
                if (maps[i].mapName == "de_inferno")
                {
                    box.Items.Add("Inferno " + maps[i].mode);
                }
                if (maps[i].mapName == "de_nuke")
                {
                    box.Items.Add("Nuke " + maps[i].mode);
                }
                if (maps[i].mapName == "de_train")
                {
                    box.Items.Add("Train " + maps[i].mode);
                }
                if (maps[i].mapName == "de_overpass")
                {
                    box.Items.Add("Overpass " + maps[i].mode);
                }
            }
        }
        #region Login2
        // 
        public void ShowInfoProfile2Tab(Profile acc)
        {
            if (isLogin2Loaded)
            {
                for (int i = 0; i < profile2.friends.Count; i++)
                {
                    if (profile2.friends[i] != null)
                    {
                        friendsBoxProfile2.Items.Add(profile2.friends[i]);
                    }
                }

                avatarProfile2.Image = profile2.imgAvatar;
                levelProfile2.Text = profile2.faceit_level.ToString();
                eloProfile2.Text = profile2.faceit_elo.ToString();
                nicknameProfile2.Text = profile2.nickname;
                countryProfile2.Text = profile2.country;
                membershipProfile2.Text = profile2.membership_type;
                steamid64Profile2.Text = profile2.steamid64;

                avgHSProfile2.Text = profile2.avgHS.ToString() + "%";
                avgKDProfile2.Text = profile2.avgKD.ToString();
                curWinStrikeProfile2.Text = profile2.currrentWinStreak.ToString();
                longestWinStrikeProfile2.Text = profile2.longestWinStreak.ToString();
                matchesProfile2.Text = profile2.matches.ToString();
                winMatchesProfile2.Text = profile2.winMatches.ToString();
                winRateProfile2.Text = profile2.winRate.ToString() + "%";
                LoadMapInComboBox(map1BoxProfile2, profile2.maps);
                LoadMapInComboBox(map2BoxProfile2, profile2.maps);
                map1BoxProfile2.SelectedIndex = 0;
                map2BoxProfile2.SelectedIndex = 0;
                metroPanel4.Visible = true;
            }   
            
        }
        // Обновить информацию о map1
        public void UpdateMapBoxProfile2()
        {
            int index = map1BoxProfile2.SelectedIndex;
            modeProfile2Map1.Text = profile2.maps[index].mode;
            matchesProfile2Map1.Text = profile2.maps[index].matches.ToString();
            winsProfile2Map1.Text = profile2.maps[index].wins.ToString();
            winRateProfile2Map1.Text = profile2.maps[index].winRate.ToString() + "%";
            roundsProfile2Map1.Text = profile2.maps[index].rounds.ToString();
            killsProfile2Map1.Text = profile2.maps[index].kills.ToString();
            deathProfile2Map1.Text = profile2.maps[index].death.ToString();
            kdRatioProfile2Map1.Text = profile2.maps[index].kdratio.ToString();
            krRatioProfile2Map1.Text = profile2.maps[index].krratio.ToString();
            mvpsProfile2Map1.Text = profile2.maps[index].mvps.ToString();
            totalHSProfile2Map1.Text = profile2.maps[index].totalHeadshots.ToString();
            assistsProfile2Map1.Text = profile2.maps[index].assist.ToString();
            avgKDRatioProfile2Map1.Text = profile2.maps[index].avgKDr.ToString();
            avgKRRatioProfile2Map1.Text = profile2.maps[index].avgKRr.ToString();
            pentaKillsProfile2Map1.Text = profile2.maps[index].pentaKills.ToString();
            avgPentaKillsProfile2Map1.Text = profile2.maps[index].avgPentaK.ToString();
            quadroKillsProfile2Map1.Text = profile2.maps[index].quadroKills.ToString();
            avgQuadroKillsProfile2Map1.Text = profile2.maps[index].avgQuadroK.ToString();
            tripleKillsProfile2Map1.Text = profile2.maps[index].tripleKills.ToString();
            avgTripleKillsProfile2Map1.Text = profile2.maps[index].avgTripleK.ToString();
            hsPerMatchProfile2Map1.Text = profile2.maps[index].hsPerMatch.ToString();
            avgKillsProfile2Map1.Text = profile2.maps[index].avgKills.ToString();
            avgDeathProfile2Map1.Text = profile2.maps[index].avgDeath.ToString();
            avgMVPsProfile2Map1.Text = profile2.maps[index].avgMVPs.ToString();
            avgAssistsProfile2Map1.Text = profile2.maps[index].avgAssist.ToString();
            avgHSsProfile2Map1.Text = profile2.maps[index].avgHS.ToString();
        }
        // Обновить информацию о map2
        public void UpdateMap2BoxProfile2()
        {
            int index = map2BoxProfile2.SelectedIndex;
            modeProfile2Map2.Text = profile2.maps[index].mode;
            matchesProfile2Map2.Text = profile2.maps[index].matches.ToString();
            winsProfile2Map2.Text = profile2.maps[index].wins.ToString();
            winRateProfile2Map2.Text = profile2.maps[index].winRate.ToString() + "%";
            roundsProfile2Map2.Text = profile2.maps[index].rounds.ToString();
            killsProfile2Map2.Text = profile2.maps[index].kills.ToString();
            deathProfile2Map2.Text = profile2.maps[index].death.ToString();
            kdRatioProfile2Map2.Text = profile2.maps[index].kdratio.ToString();
            krRatioProfile2Map2.Text = profile2.maps[index].krratio.ToString();
            mvpsProfile2Map2.Text = profile2.maps[index].mvps.ToString();
            totalHSProfile2Map2.Text = profile2.maps[index].totalHeadshots.ToString();
            assistsProfile2Map2.Text = profile2.maps[index].assist.ToString();
            avgKDRatioProfile2Map2.Text = profile2.maps[index].avgKDr.ToString();
            avgKRRatioProfile2Map2.Text = profile2.maps[index].avgKRr.ToString();
            pentaKillsProfile2Map2.Text = profile2.maps[index].pentaKills.ToString();
            avgPentaKillsProfile2Map2.Text = profile2.maps[index].avgPentaK.ToString();
            quadroKillsProfile2Map2.Text = profile2.maps[index].quadroKills.ToString();
            avgQuadroKillsProfile2Map2.Text = profile2.maps[index].avgQuadroK.ToString();
            tripleKillsProfile2Map2.Text = profile2.maps[index].tripleKills.ToString();
            avgTripleKillsProfile2Map2.Text = profile2.maps[index].avgTripleK.ToString();
            hsPerMatchProfile2Map2.Text = profile2.maps[index].hsPerMatch.ToString();
            avgKillsProfile2Map2.Text = profile2.maps[index].avgKills.ToString();
            avgDeathProfile2Map2.Text = profile2.maps[index].avgDeath.ToString();
            avgMVPsProfile2Map2.Text = profile2.maps[index].avgMVPs.ToString();
            avgAssistsProfile2Map2.Text = profile2.maps[index].avgAssist.ToString();
            avgHSsProfile2Map2.Text = profile2.maps[index].avgHS.ToString();
        }
        // Показать информацию о map1
        private void map1BoxProfile2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMapBoxProfile2();
            loginMapBox1Profile2_defaultStyle();
        }
        // Показать информацию о map2
        private void map2BoxProfile2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMap2BoxProfile2();
            loginMapBox2Profile2_defaultStyle();
        }
        // Очистить табло логин1
        public void ClearProfile2Tab()
        {
            friendsBoxProfile2.Items.Clear();
            map1BoxProfile2.Items.Clear();
            map2BoxProfile2.Items.Clear();
        }
        // 
        private void compareMapsProfile2_Click(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile2_defaultStyle();
            loginMapBox1Profile2_defaultStyle();
            loginMapBox2Profile2_defaultStyle();
            CompareMap1andMap2Profile2_Style();
            SelectBestMapProfile2();
        }
        // Загрузка login2
        private void metroButton4_Click(object sender, EventArgs e)
        {
            isLogin2Loaded = false;
            CheckOnLoadedLogins();
            metroPanel4.Visible = false;
            ClearProfile2Tab();
            profile2 = new Profile();
            profile2.SetInfo(login2.Text);
            if (profile2.json.HasValues & profile2.json.Type != JTokenType.Null)
            {
                if (profile2.dec.HasValues & profile2.dec.Type != JTokenType.Null)
                {
                    friendsBoxProfile2.Enabled = true;
                    getFriendInfoProfile2.Enabled = true;
                    isLogin2Loaded = true;
                    ShowInfoProfile2Tab(profile2);
                    CheckOnLoadedLogins();
                    DisableStatsCompareProfile();
                    loginProfileProfile2_defaultStyle();
                    loginMapBox1Profile2_defaultStyle();
                    loginMapBox2Profile2_defaultStyle();
                    metroTabControl1.SelectTab(1);
                    if (profile2.friends.Count != 0)
                    {
                        friendsBoxProfile2.Enabled = true;
                        getFriendInfoProfile2.Enabled = true;
                        friendsBoxProfile2.SelectedIndex = 0;
                    }
                    else
                    {
                        friendsBoxProfile2.Enabled = false;
                        getFriendInfoProfile2.Enabled = false;
                    }
                }
            }

        }

        #endregion
        
        #region Match

        // Открыть в браузере и скачать демо файл
        private void downloadDemo1_Click(object sender, EventArgs e)
        {
            Process.Start(match.demoURL.ToString());
        }
        // Открыть в браузере комнату матча
        private void OpenMatchInWeb1_Click(object sender, EventArgs e)
        {
            Process.Start(match.matchURL.ToString());
        }
        // Открытие профиля в браузере
        private void mNickname1_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamA.leader.faceit_profile.ToString());
        }
        // Показать информацию о матче
        public void ShowInfoMatch()
        {
            if (match.TeamA.leader != null)
            {
                teamnameA.Text = match.TeamA.teamName;
                teamnameB.Text = match.TeamB.teamName;
                scoreTeamA.Text = match.scoreTeamA;
                scoreTeamB.Text = match.scoreTeamB;
                statusMatch1.Text = match.status;
                mapName1.Text = match.map;
                gameType1.Text = match.type_match;
                matchBestOf1.Text = match.bestOf.ToString();
                countryServer1.Text = match.serverCountry;

                int memberOldA = 0;
                List<Member> membOldA = new List<Member>();
                membOldA.Add(match.TeamA.leader);
                membOldA.Add(match.TeamA.member2);
                membOldA.Add(match.TeamA.member3);
                membOldA.Add(match.TeamA.member4);
                membOldA.Add(match.TeamA.member5);
                for(int i = 0; i < membOldA.Count; i++)
                {
                    if(membOldA[i].faceit_elo != 0)
                    {
                        memberOldA += 1;
                    }
                }

                int memberOldB = 0;
                List<Member> membOldB = new List<Member>();
                membOldB.Add(match.TeamB.leader);
                membOldB.Add(match.TeamB.member2);
                membOldB.Add(match.TeamB.member3);
                membOldB.Add(match.TeamB.member4);
                membOldB.Add(match.TeamB.member5);
                for (int i = 0; i < membOldB.Count; i++)
                {
                    if (membOldB[i].faceit_elo != 0)
                    {
                        memberOldB += 1;
                    }
                }

                int avgeloteama = 0;
                avgeloteama = (match.TeamA.leader.faceit_elo + match.TeamA.member2.faceit_elo + match.TeamA.member3.faceit_elo + match.TeamA.member4.faceit_elo + match.TeamA.member5.faceit_elo) / memberOldA;
                double avghsteama = 0;
                avghsteama = (match.TeamA.leader.avgHS + match.TeamA.member2.avgHS + match.TeamA.member3.avgHS + match.TeamA.member4.avgHS + match.TeamA.member5.avgHS) / memberOldA;
                double avgkdteama = 0;
                avgkdteama = (match.TeamA.leader.avgKDr + match.TeamA.member2.avgKDr + match.TeamA.member3.avgKDr + match.TeamA.member4.avgKDr + match.TeamA.member5.avgKDr) / memberOldA;
                int avgrankingteama = 0;
                avgrankingteama = (match.TeamA.leader.ranking + match.TeamA.member2.ranking + match.TeamA.member3.ranking + match.TeamA.member4.ranking + match.TeamA.member5.ranking) / memberOldA;
                double avgmatchesteama = 0;
                avgmatchesteama = (match.TeamA.leader.matches + match.TeamA.member2.matches + match.TeamA.member3.matches + match.TeamA.member4.matches + match.TeamA.member5.matches) / memberOldA;
                double avgwinrateteama = 0;
                avgwinrateteama = (match.TeamA.leader.winRate + match.TeamA.member2.winRate + match.TeamA.member3.winRate + match.TeamA.member4.winRate + match.TeamA.member5.winRate) / memberOldA;

                int avgeloteamb = 0;
                avgeloteamb = (match.TeamB.leader.faceit_elo + match.TeamB.member2.faceit_elo + match.TeamB.member3.faceit_elo + match.TeamB.member4.faceit_elo + match.TeamB.member5.faceit_elo) / memberOldB;
                double avghsteamb = 0;
                avghsteamb = (match.TeamB.leader.avgHS + match.TeamB.member2.avgHS + match.TeamB.member3.avgHS + match.TeamB.member4.avgHS + match.TeamB.member5.avgHS) / memberOldB;
                double avgkdteamb = 0;
                avgkdteamb = (match.TeamB.leader.avgKDr + match.TeamB.member2.avgKDr + match.TeamB.member3.avgKDr + match.TeamB.member4.avgKDr + match.TeamB.member5.avgKDr) / memberOldB;
                int avgrankingteamb = 0;
                avgrankingteamb = (match.TeamB.leader.ranking + match.TeamB.member2.ranking + match.TeamB.member3.ranking + match.TeamB.member4.ranking + match.TeamB.member5.ranking) / memberOldB;
                double avgmatchesteamb = 0;
                avgmatchesteamb = (match.TeamB.leader.matches + match.TeamB.member2.matches + match.TeamB.member3.matches + match.TeamB.member4.matches + match.TeamB.member5.matches) / memberOldB;
                double avgwinrateteamb = 0;
                avgwinrateteamb = (match.TeamB.leader.winRate + match.TeamB.member2.winRate + match.TeamB.member3.winRate + match.TeamB.member4.winRate + match.TeamB.member5.winRate) / memberOldB;

                avgEloTeamA.Text = avgeloteama.ToString();
                avgHSTeamA.Text = avghsteama.ToString() + "%";
                avgKDTeamA.Text = avgkdteama.ToString();
                avgRankingTeamA.Text = avgrankingteama.ToString();
                avgMatchesTeamA.Text = avgmatchesteama.ToString();
                avgWinRateTeamA.Text = avgwinrateteama.ToString() + "%";

                avgEloTeamB.Text = avgeloteamb.ToString();
                avgHSTeamB.Text = avghsteamb.ToString() + "%";
                avgKDTeamB.Text = avgkdteamb.ToString();
                avgRankingTeamB.Text = avgrankingteamb.ToString();
                avgMatchesTeamB.Text = avgmatchesteamb.ToString();
                avgWinRateTeamB.Text = avgwinrateteamb.ToString() + "%";

                if (avgeloteama > avgeloteamb)
                {
                    avgEloTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgEloTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avgeloteama < avgeloteamb)
                {
                    avgEloTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgEloTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgEloTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgEloTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                if (avghsteama > avghsteamb)
                {
                    avgHSTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgHSTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avghsteama < avghsteamb)
                {
                    avgHSTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgHSTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgHSTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgHSTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                if (avgkdteama > avgkdteamb)
                {
                    avgKDTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgKDTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avgkdteama < avgkdteamb)
                {
                    avgKDTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgKDTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgKDTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgKDTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                if (avgrankingteama < avgrankingteamb)
                {
                    avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avgrankingteama > avgrankingteamb)
                {
                    avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                if (avgmatchesteama > avgmatchesteamb)
                {
                    avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avgmatchesteama < avgmatchesteamb)
                {
                    avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                if (avgwinrateteama > avgwinrateteamb)
                {
                    avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Red;
                    avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Green;
                }
                else if (avgwinrateteama < avgwinrateteamb)
                {
                    avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Green;
                    avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Red;
                }
                else
                {
                    avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                    avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Blue;
                }

                //Team A
                //Leader Team A
                mNickname1.Text = match.TeamA.leader.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamA.leader.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry1.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel1.Image = GetImageLevel(match.TeamA.leader.faceit_level);
                mAvatar1.Image = match.TeamA.leader.avatar;
                mMatches1.Text = "Matches: " + match.TeamA.leader.matches.ToString();
                mWinRate1.Text = "Win Rate: " + match.TeamA.leader.winRate.ToString() + "%";
                mElo1.Text = match.TeamA.leader.faceit_elo.ToString() + " ELO";
                mJoinType1.Text = match.TeamA.leader.join_type;
                mAvgHS1.Text = "Avg. HS: " + match.TeamA.leader.avgHS.ToString() + "%";
                mKD1.Text = "Avg. KD: " + match.TeamA.leader.avgKDr;
                mRanking1.Text = "Ranking: " + match.TeamA.leader.ranking;
                for (int i = 0; i < match.TeamA.leader.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamA.leader.select_members_id[i], false);
                    mLobby1.Items.Add(dec["nickname"].ToString());
                }
                mLobby1.SelectedIndex = 0;
                if (match.TeamA.leader.join_type == "mix")
                {
                    mJoinType1.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType1.Style = MetroFramework.MetroColorStyle.Green;
                }


                //Member2
                mNickname2.Text = match.TeamA.member2.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamA.member2.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry2.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel2.Image = GetImageLevel(match.TeamA.member2.faceit_level);
                mAvatar2.Image = match.TeamA.member2.avatar;
                mMatches2.Text = "Matches: " + match.TeamA.member2.matches.ToString();
                mWinRate2.Text = "Win Rate: " + match.TeamA.member2.winRate.ToString() + "%";
                mElo2.Text = match.TeamA.member2.faceit_elo.ToString() + " ELO";
                mJoinType2.Text = match.TeamA.member2.join_type;
                mAvgHS2.Text = "Avg. HS: " + match.TeamA.member2.avgHS.ToString() + "%";
                mKD2.Text = "Avg. KD: " + match.TeamA.member2.avgKDr;
                mRanking2.Text = "Ranking: " + match.TeamA.member2.ranking;
                for (int i = 0; i < match.TeamA.member2.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamA.member2.select_members_id[i], false);
                    mLobby2.Items.Add(dec["nickname"].ToString());
                }
                mLobby2.SelectedIndex = 0;
                if (match.TeamA.member2.join_type == "mix")
                {
                    mJoinType2.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType2.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Member3
                mNickname3.Text = match.TeamA.member3.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamA.member3.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry3.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel3.Image = GetImageLevel(match.TeamA.member3.faceit_level);
                mAvatar3.Image = match.TeamA.member3.avatar;
                mMatches3.Text = "Matches: " + match.TeamA.member3.matches.ToString();
                mWinRate3.Text = "Win Rate: " + match.TeamA.member3.winRate.ToString() + "%";
                mElo3.Text = match.TeamA.member3.faceit_elo.ToString() + " ELO";
                mJoinType3.Text = match.TeamA.member3.join_type;
                mAvgHS3.Text = "Avg. HS: " + match.TeamA.member3.avgHS.ToString() + "%";
                mKD3.Text = "Avg. KD: " + match.TeamA.member3.avgKDr;
                mRanking3.Text = "Ranking: " + match.TeamA.member3.ranking;
                for (int i = 0; i < match.TeamA.member3.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamA.member3.select_members_id[i], false);
                    mLobby3.Items.Add(dec["nickname"].ToString());
                }
                mLobby3.SelectedIndex = 0;
                if (match.TeamA.member3.join_type == "mix")
                {
                    mJoinType3.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType3.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Member4
                mNickname4.Text = match.TeamA.member4.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamA.member4.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry4.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel4.Image = GetImageLevel(match.TeamA.member4.faceit_level);
                mAvatar4.Image = match.TeamA.member4.avatar;
                mMatches4.Text = "Matches: " + match.TeamA.member4.matches.ToString();
                mWinRate4.Text = "Win Rate: " + match.TeamA.member4.winRate.ToString() + "%";
                mElo4.Text = match.TeamA.member4.faceit_elo.ToString() + " ELO";
                mJoinType4.Text = match.TeamA.member4.join_type;
                mAvgHS4.Text = "Avg. HS: " + match.TeamA.member4.avgHS.ToString() + "%";
                mKD4.Text = "Avg. KD: " + match.TeamA.member4.avgKDr;
                mRanking4.Text = "Ranking: " + match.TeamA.member4.ranking;
                for (int i = 0; i < match.TeamA.member4.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamA.member4.select_members_id[i], false);
                    mLobby4.Items.Add(dec["nickname"].ToString());
                }
                mLobby4.SelectedIndex = 0;
                if (match.TeamA.member4.join_type == "mix")
                {
                    mJoinType4.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType4.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Member5
                mNickname5.Text = match.TeamA.member5.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamA.member5.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry5.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel5.Image = GetImageLevel(match.TeamA.member5.faceit_level);
                mAvatar5.Image = match.TeamA.member5.avatar;
                mMatches5.Text = "Matches: " + match.TeamA.member5.matches.ToString();
                mWinRate5.Text = "Win Rate: " + match.TeamA.member5.winRate.ToString() + "%";
                mElo5.Text = match.TeamA.member5.faceit_elo.ToString() + " ELO";
                mJoinType5.Text = match.TeamA.member5.join_type;
                mAvgHS5.Text = "Avg. HS: " + match.TeamA.member5.avgHS.ToString() + "%";
                mKD5.Text = "Avg. KD: " + match.TeamA.member5.avgKDr;
                mRanking5.Text = "Ranking: " + match.TeamA.member5.ranking;
                for (int i = 0; i < match.TeamA.member5.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamA.member5.select_members_id[i], false);
                    mLobby5.Items.Add(dec["nickname"].ToString());
                }
                mLobby5.SelectedIndex = 0;
                if (match.TeamA.member5.join_type == "mix")
                {
                    mJoinType5.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType5.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Team B
                //Leader - Team B
                mNickname6.Text = match.TeamB.leader.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamB.leader.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry6.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel6.Image = GetImageLevel(match.TeamB.leader.faceit_level);
                mAvatar6.Image = match.TeamB.leader.avatar;
                mMatches6.Text = "Matches: " + match.TeamB.leader.matches.ToString();
                mWinRate6.Text = "Win Rate: " + match.TeamB.leader.winRate.ToString() + "%";
                mElo6.Text = match.TeamB.leader.faceit_elo.ToString() + " ELO";
                mJoinType6.Text = match.TeamB.leader.join_type;
                mAvgHS6.Text = "Avg. HS: " + match.TeamB.leader.avgHS.ToString() + "%";
                mKD6.Text = "Avg. KD: " + match.TeamB.leader.avgKDr;
                mRanking6.Text = "Ranking: " + match.TeamB.leader.ranking;
                for (int i = 0; i < match.TeamB.leader.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamB.leader.select_members_id[i], false);
                    mLobby6.Items.Add(dec["nickname"].ToString());
                }
                mLobby6.SelectedIndex = 0;
                if (match.TeamB.leader.join_type == "mix")
                {
                    mJoinType6.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType6.Style = MetroFramework.MetroColorStyle.Green;
                }
                //Member2
                mNickname7.Text = match.TeamB.member2.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamB.member2.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry7.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel7.Image = GetImageLevel(match.TeamB.member2.faceit_level);
                mAvatar7.Image = match.TeamB.member2.avatar;
                mMatches7.Text = "Matches: " + match.TeamB.member2.matches.ToString();
                mWinRate7.Text = "Win Rate: " + match.TeamB.member2.winRate.ToString() + "%";
                mElo7.Text = match.TeamB.member2.faceit_elo.ToString() + " ELO";
                mJoinType7.Text = match.TeamB.member2.join_type;
                mAvgHS7.Text = "Avg. HS: " + match.TeamB.member2.avgHS.ToString() + "%";
                mKD7.Text = "Avg. KD: " + match.TeamB.member2.avgKDr;
                mRanking7.Text = "Ranking: " + match.TeamB.member2.ranking;
                for (int i = 0; i < match.TeamB.member2.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamB.member2.select_members_id[i], false);
                    mLobby7.Items.Add(dec["nickname"].ToString());
                }
                mLobby7.SelectedIndex = 0;
                if (match.TeamB.member2.join_type == "mix")
                {
                    mJoinType7.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType7.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Member3
                mNickname8.Text = match.TeamB.member3.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamB.member3.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry8.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel8.Image = GetImageLevel(match.TeamB.member3.faceit_level);
                mAvatar8.Image = match.TeamB.member3.avatar;
                mMatches8.Text = "Matches: " + match.TeamB.member3.matches.ToString();
                mWinRate8.Text = "Win Rate: " + match.TeamB.member3.winRate.ToString() + "%";
                mElo8.Text = match.TeamB.member3.faceit_elo.ToString() + " ELO";
                mJoinType8.Text = match.TeamB.member3.join_type;
                mAvgHS8.Text = "Avg. HS: " + match.TeamB.member3.avgHS.ToString() + "%";
                mKD8.Text = "Avg. KD: " + match.TeamB.member3.avgKDr;
                mRanking8.Text = "Ranking: " + match.TeamB.member3.ranking;
                for (int i = 0; i < match.TeamB.member3.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamB.member3.select_members_id[i], false);
                    mLobby8.Items.Add(dec["nickname"].ToString());
                }
                mLobby8.SelectedIndex = 0;
                if (match.TeamB.member3.join_type == "mix")
                {
                    mJoinType8.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType8.Style = MetroFramework.MetroColorStyle.Green;
                }
                //Member4
                mNickname9.Text = match.TeamB.member4.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamB.member4.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry9.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel9.Image = GetImageLevel(match.TeamB.member4.faceit_level);
                mAvatar9.Image = match.TeamB.member4.avatar;
                mMatches9.Text = "Matches: " + match.TeamB.member4.matches.ToString();
                mWinRate9.Text = "Win Rate: " + match.TeamB.member4.winRate.ToString() + "%";
                mElo9.Text = match.TeamB.member4.faceit_elo.ToString() + " ELO";
                mJoinType9.Text = match.TeamB.member4.join_type;
                mAvgHS9.Text = "Avg. HS: " + match.TeamB.member4.avgHS.ToString() + "%";
                mKD9.Text = "Avg. KD: " + match.TeamB.member4.avgKDr;
                mRanking9.Text = "Ranking: " + match.TeamB.member4.ranking;
                for (int i = 0; i < match.TeamB.member4.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamB.member4.select_members_id[i], false);
                    mLobby9.Items.Add(dec["nickname"].ToString());
                }
                mLobby9.SelectedIndex = 0;
                if (match.TeamB.member4.join_type == "mix")
                {
                    mJoinType9.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType9.Style = MetroFramework.MetroColorStyle.Green;
                }

                //Member5
                mNickname10.Text = match.TeamB.member5.nickname;
                try
                {
                    WebRequest web2 = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + match.TeamB.member5.country.ToUpper() + ".png");
                    WebResponse web3 = web2.GetResponse();
                    mCountry10.Image = Image.FromStream(web3.GetResponseStream());
                }
                catch (Exception)
                {

                }
                mLevel10.Image = GetImageLevel(match.TeamB.member5.faceit_level);
                mAvatar10.Image = match.TeamB.member5.avatar;
                mMatches10.Text = "Matches: " + match.TeamB.member5.matches.ToString();
                mWinRate10.Text = "Win Rate: " + match.TeamB.member5.winRate.ToString() + "%";
                mElo10.Text = match.TeamB.member5.faceit_elo.ToString() + " ELO";
                mJoinType10.Text = match.TeamB.member5.join_type;
                mAvgHS10.Text = "Avg. HS: " + match.TeamB.member5.avgHS.ToString() + "%";
                mKD10.Text = "Avg. KD: " + match.TeamB.member5.avgKDr;
                mRanking10.Text = "Ranking: " + match.TeamB.member5.ranking;
                for (int i = 0; i < match.TeamB.member5.select_members_id.Count; i++)
                {
                    JObject dec = GetProfile(match.TeamB.member5.select_members_id[i], false);
                    mLobby10.Items.Add(dec["nickname"].ToString());
                }
                mLobby10.SelectedIndex = 0;
                if (match.TeamB.member5.join_type == "mix")
                {
                    mJoinType10.Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    mJoinType10.Style = MetroFramework.MetroColorStyle.Green;
                }
            }
        }

        #endregion

        #region Style

        #region Profile1
        // Загрузка обычных стилей о map1Profile2
        public void loginMapBox1Profile1_defaultStyle()
        {
            winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;

        }
        // Загрузка обычных стилей о map2Profile2
        public void loginMapBox2Profile1_defaultStyle()
        {
            modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузка обычных стилей о profile
        public void loginProfileProfile1_defaultStyle()
        {
            eloProfile1.Style = MetroFramework.MetroColorStyle.Black;
            levelProfile1.Style = MetroFramework.MetroColorStyle.Black;
            countryProfile1.Style = MetroFramework.MetroColorStyle.Black;
            membershipProfile1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSProfile1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDProfile1.Style = MetroFramework.MetroColorStyle.Black;
            curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Black;
            longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1.Style = MetroFramework.MetroColorStyle.Black;
            winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузить стили сравнения карт
        public void CompareMap1andMap2Profile1_Style()
        {
            int index1 = map1BoxProfile1.SelectedIndex;
            int index2 = map2BoxProfile1.SelectedIndex;
            Map map1 = profile.maps[index1];
            Map map2 = profile.maps[index2];

            if (map1.wins > map2.wins)
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.winRate > map2.winRate)
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.winRate < map2.winRate)
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kdratio > map2.kdratio)
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kdratio < map2.kdratio)
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgHS > map2.avgHS)
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgHS < map2.avgHS)
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.death < map2.death)
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.death > map2.death)
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.krratio > map2.krratio)
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.krratio < map2.krratio)
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assist > map2.assist)
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.assist < map2.assist)
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.pentaKills > map2.pentaKills)
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                metrolabel.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.pentaKills < map2.pentaKills)
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                metrolabel.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadroKills > map2.quadroKills)
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.quadroKills < map2.quadroKills)
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.tripleKills > map2.tripleKills)
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.tripleKills < map2.tripleKills)
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.hsPerMatch > map2.hsPerMatch)
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.hsPerMatch < map2.hsPerMatch)
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKills > map2.avgKills)
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKills < map2.avgKills)
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgDeath < map2.avgDeath)
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgDeath > map2.avgDeath)
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.totalHeadshots > map2.totalHeadshots)
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.totalHeadshots < map2.totalHeadshots)
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgAssist > map2.avgAssist)
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgAssist < map2.avgAssist)
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgMVPs > map2.avgMVPs)
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgMVPs < map2.avgMVPs)
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgPentaK > map2.avgPentaK)
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgPentaK < map2.avgPentaK)
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgQuadroK > map2.avgQuadroK)
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgQuadroK < map2.avgQuadroK)
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgTripleK > map2.avgTripleK)
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgTripleK < map2.avgTripleK)
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKRr > map2.avgKRr)
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKRr < map2.avgKRr)
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKDr > map2.avgKDr)
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKDr < map2.avgKDr)
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

        }
        // Сравнение карт
        public void SelectBestMapProfile1()
        {
            Map map1 = profile.maps[map1BoxProfile1.SelectedIndex];
            Map map2 = profile.maps[map2BoxProfile1.SelectedIndex];

            int indexMap1 = 0;
            int indexMap2 = 0;

            if (map1.matches < map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.matches > map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.wins < map2.wins)
            {
                indexMap2 += 1;
            }
            else if (map1.wins > map2.wins)
            {
                indexMap1 += 1;
            }

            if (map1.winRate < map2.winRate)
            {
                indexMap2 += 1;
            }
            else if (map1.winRate > map2.winRate)
            {
                indexMap1 += 1;
            }

            if (map1.kdratio < map2.kdratio)
            {
                indexMap2 += 1;
            }
            else if (map1.kdratio > map2.kdratio)
            {
                indexMap1 += 1;
            }

            if (map1.avgHS < map2.avgHS)
            {
                indexMap2 += 1;
            }
            else if (map1.avgHS > map2.avgHS)
            {
                indexMap1 += 1;
            }

            if (map1.rounds < map2.rounds)
            {
                indexMap2 += 1;
            }
            else if (map1.rounds > map2.rounds)
            {
                indexMap1 += 1;
            }

            if (map1.kills < map2.kills)
            {
                indexMap2 += 1;
            }
            else if (map1.kills > map2.kills)
            {
                indexMap1 += 1;
            }

            if (map1.death > map2.death)
            {
                indexMap2 += 1;
            }
            else if (map1.death < map2.death)
            {
                indexMap1 += 1;
            }

            if (map1.krratio < map2.krratio)
            {
                indexMap2 += 1;
            }
            else if (map1.krratio > map2.krratio)
            {
                indexMap1 += 1;
            }

            if (map1.mvps < map2.mvps)
            {
                indexMap2 += 1;
            }
            else if (map1.mvps > map2.mvps)
            {
                indexMap1 += 1;
            }

            if (map1.assist < map2.assist)
            {
                indexMap2 += 1;
            }
            else if (map1.assist > map2.assist)
            {
                indexMap1 += 1;
            }

            if (map1.pentaKills < map2.pentaKills)
            {
                indexMap2 += 1;
            }
            else if (map1.pentaKills > map2.pentaKills)
            {
                indexMap1 += 1;
            }

            if (map1.quadroKills < map2.quadroKills)
            {
                indexMap2 += 1;
            }
            else if (map1.quadroKills > map2.quadroKills)
            {
                indexMap1 += 1;
            }

            if (map1.tripleKills < map2.tripleKills)
            {
                indexMap2 += 1;
            }
            else if (map1.tripleKills > map2.tripleKills)
            {
                indexMap1 += 1;
            }

            if (map1.hsPerMatch < map2.hsPerMatch)
            {
                indexMap2 += 1;
            }
            else if (map1.hsPerMatch > map2.hsPerMatch)
            {
                indexMap1 += 1;
            }

            if (map1.avgKills < map2.avgKills)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKills > map2.avgKills)
            {
                indexMap1 += 1;
            }

            if (map1.avgDeath > map2.avgDeath)
            {
                indexMap2 += 1;
            }
            else if (map1.avgDeath < map2.avgDeath)
            {
                indexMap1 += 1;
            }

            if (map1.avgAssist < map2.avgAssist)
            {
                indexMap2 += 1;
            }
            else if (map1.avgAssist > map2.avgAssist)
            {
                indexMap1 += 1;
            }

            if (map1.avgMVPs < map2.avgMVPs)
            {
                indexMap2 += 1;
            }
            else if (map1.avgMVPs > map2.avgMVPs)
            {
                indexMap1 += 1;
            }

            if (map1.avgPentaK < map2.avgPentaK)
            {
                indexMap2 += 1;
            }
            else if (map1.avgPentaK > map2.avgPentaK)
            {
                indexMap1 += 1;
            }

            if (map1.avgQuadroK < map2.avgQuadroK)
            {
                indexMap2 += 1;
            }
            else if (map1.avgQuadroK > map2.avgQuadroK)
            {
                indexMap1 += 1;
            }

            if (map1.avgTripleK < map2.avgTripleK)
            {
                indexMap2 += 1;
            }
            else if (map1.avgTripleK > map2.avgTripleK)
            {
                indexMap1 += 1;
            }

            if (map1.avgKDr < map2.avgKDr)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKDr > map2.avgKDr)
            {
                indexMap1 += 1;
            }

            if (map1.avgKRr < map2.avgKRr)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKRr > map2.avgKRr)
            {
                indexMap1 += 1;
            }

            if (indexMap1 > indexMap2)
            {
                bestMapProfile1Map1.Text = "Best";
                bestMapProfile1Map2.Text = "Worst";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap1 < indexMap2)
            {
                bestMapProfile1Map1.Text = "Worst";
                bestMapProfile1Map2.Text = "Best";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                bestMapProfile1Map1.Text = "Best";
                bestMapProfile1Map2.Text = "Best";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            metro.Visible = true;
            bestMapProfile1Map1.Visible = true;
            bestMapProfile1Map2.Visible = true;
        }
        #endregion

        #region Profile2

        // Загрузка обычных стилей о map1Profile2
        public void loginMapBox1Profile2_defaultStyle()
        {
            winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;

        }
        // Загрузка обычных стилей о map2Profile2
        public void loginMapBox2Profile2_defaultStyle()
        {
            modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузка обычных стилей о profile
        public void loginProfileProfile2_defaultStyle()
        {
            eloProfile2.Style = MetroFramework.MetroColorStyle.Black;
            levelProfile2.Style = MetroFramework.MetroColorStyle.Black;
            countryProfile2.Style = MetroFramework.MetroColorStyle.Black;
            membershipProfile2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSProfile2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDProfile2.Style = MetroFramework.MetroColorStyle.Black;
            curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Black;
            longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2.Style = MetroFramework.MetroColorStyle.Black;
            winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузить стили сравнения карт
        public void CompareMap1andMap2Profile2_Style()
        {
            int index1 = map1BoxProfile2.SelectedIndex;
            int index2 = map2BoxProfile2.SelectedIndex;
            Map map1 = profile2.maps[index1];
            Map map2 = profile2.maps[index2];

            if (map1.wins > map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.winRate > map2.winRate)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.winRate < map2.winRate)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kdratio > map2.kdratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kdratio < map2.kdratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgHS > map2.avgHS)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgHS < map2.avgHS)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.death < map2.death)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.death > map2.death)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.krratio > map2.krratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.krratio < map2.krratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assist > map2.assist)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.assist < map2.assist)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.pentaKills > map2.pentaKills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.pentaKills < map2.pentaKills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadroKills > map2.quadroKills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.quadroKills < map2.quadroKills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.tripleKills > map2.tripleKills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.tripleKills < map2.tripleKills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.hsPerMatch > map2.hsPerMatch)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.hsPerMatch < map2.hsPerMatch)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKills > map2.avgKills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKills < map2.avgKills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgDeath < map2.avgDeath)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgDeath > map2.avgDeath)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.totalHeadshots > map2.totalHeadshots)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.totalHeadshots < map2.totalHeadshots)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgAssist > map2.avgAssist)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgAssist < map2.avgAssist)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgMVPs > map2.avgMVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgMVPs < map2.avgMVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgPentaK > map2.avgPentaK)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgPentaK < map2.avgPentaK)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgQuadroK > map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgQuadroK < map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgTripleK > map2.avgTripleK)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgTripleK < map2.avgTripleK)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKRr > map2.avgKRr)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKRr < map2.avgKRr)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKDr > map2.avgKDr)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.avgKDr < map2.avgKDr)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

        }
        // Сравнение карт
        public void SelectBestMapProfile2()
        {
            Map map1 = profile2.maps[map1BoxProfile2.SelectedIndex];
            Map map2 = profile2.maps[map2BoxProfile2.SelectedIndex];

            int indexMap1 = 0;
            int indexMap2 = 0;

            if (map1.matches < map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.matches > map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.wins/map1.matches < map2.wins / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.wins / map1.matches > map2.wins / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.winRate / map1.matches < map2.winRate / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.winRate / map1.matches > map2.winRate / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.kdratio / map1.matches / map1.matches < map2.kdratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.kdratio / map1.matches / map1.matches > map2.kdratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgHS / map1.matches < map2.avgHS / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgHS / map1.matches > map2.avgHS / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.rounds / map1.matches / map1.matches < map2.rounds / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.rounds / map1.matches / map1.matches > map2.rounds / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.kills / map1.matches < map2.kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.kills / map1.matches > map2.kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.death / map1.matches > map2.death / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.death / map1.matches < map2.death / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.krratio / map1.matches < map2.krratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.krratio / map1.matches > map2.krratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.mvps / map1.matches < map2.mvps / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.mvps / map1.matches > map2.mvps / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.assist / map1.matches < map2.assist / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.assist / map1.matches > map2.assist / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.pentaKills / map1.matches < map2.pentaKills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.pentaKills / map1.matches > map2.pentaKills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.quadroKills / map1.matches < map2.quadroKills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.quadroKills / map1.matches > map2.quadroKills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.tripleKills / map1.matches < map2.tripleKills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.tripleKills / map1.matches > map2.tripleKills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.hsPerMatch / map1.matches < map2.hsPerMatch / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.hsPerMatch / map1.matches > map2.hsPerMatch / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgKills / map1.matches < map2.avgKills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKills / map1.matches > map2.avgKills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgDeath / map1.matches > map2.avgDeath / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgDeath / map1.matches < map2.avgDeath / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgAssist / map1.matches < map2.avgAssist / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgAssist / map1.matches > map2.avgAssist / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgMVPs / map1.matches < map2.avgMVPs / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgMVPs / map1.matches > map2.avgMVPs / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgPentaK / map1.matches < map2.avgPentaK / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgPentaK / map1.matches > map2.avgPentaK / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgQuadroK / map1.matches < map2.avgQuadroK / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgQuadroK / map1.matches > map2.avgQuadroK / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgTripleK / map1.matches < map2.avgTripleK / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgTripleK / map1.matches > map2.avgTripleK / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgKDr / map1.matches < map2.avgKDr / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKDr / map1.matches > map2.avgKDr / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.avgKRr / map1.matches < map2.avgKRr / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.avgKRr / map1.matches > map2.avgKRr / map2.matches)
            {
                indexMap1 += 1;
            }

            if (indexMap1 > indexMap2)
            {
                bestMapProfile2Map1.Text = "Best";
                bestMapProfile2Map2.Text = "Worst";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap1 < indexMap2)
            {
                bestMapProfile2Map1.Text = "Worst";
                bestMapProfile2Map2.Text = "Best";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                bestMapProfile2Map1.Text = "Best";
                bestMapProfile2Map2.Text = "Best";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            metroLabel147.Visible = true;
            bestMapProfile2Map1.Visible = true;
            bestMapProfile2Map2.Visible = true;
        }

        #endregion

        
        public void EnableStatsCompareProfile()
        {
            metroLabel147.Visible = true;
            metro.Visible = true;
            bestMapProfile1Map1.Visible = true;
            bestMapProfile1Map2.Visible = true;
            bestMapProfile2Map1.Visible = true;
            bestMapProfile2Map2.Visible = true;

            profileProfile1Map1.Visible = true;
            profileProfile1Map2.Visible = true;
            profileProfile2Map1.Visible = true;
            profileProfile2Map2.Visible = true;

            profileAllProfile1Map1.Visible = true;
            profileAllProfile1Map2.Visible = true;
            profileAllProfile2Map1.Visible = true;
            profileAllProfile2Map2.Visible = true;

            profileMap1Profile1Map1.Visible = true;
            profileMap1Profile1Map2.Visible = true;
            profileMap1Profile2Map1.Visible = true;
            profileMap1Profile2Map2.Visible = true;

            profileMap2Profile1Map1.Visible = true;
            profileMap2Profile1Map2.Visible = true;
            profileMap2Profile2Map1.Visible = true;
            profileMap2Profile2Map2.Visible = true;

            //Для профиля #1
            metroLabel214.Visible = true;
            metroLabel213.Visible = true;
            metroLabel247.Visible = true;
            metroLabel245.Visible = true;
            metroLabel249.Visible = true;
            metroLabel253.Visible = true;

            //Для профиля #2
            metroLabel265.Visible = true;
            metroLabel272.Visible = true;
            metroLabel258.Visible = true;
            metroLabel270.Visible = true;
            metroLabel243.Visible = true;
            metroLabel266.Visible = true;
        }

        public void DisableStatsCompareProfile()
        {
            metroLabel147.Visible = false;
            metro.Visible = false;
            bestMapProfile1Map1.Visible = false;
            bestMapProfile1Map2.Visible = false;
            bestMapProfile2Map1.Visible = false;
            bestMapProfile2Map2.Visible = false;

            profileProfile1Map1.Visible = false;
            profileProfile1Map2.Visible = false;
            profileProfile2Map1.Visible = false;
            profileProfile2Map2.Visible = false;

            profileAllProfile1Map1.Visible = false;
            profileAllProfile1Map2.Visible = false;
            profileAllProfile2Map1.Visible = false;
            profileAllProfile2Map2.Visible = false;

            profileMap1Profile1Map1.Visible = false;
            profileMap1Profile1Map2.Visible = false;
            profileMap1Profile2Map1.Visible = false;
            profileMap1Profile2Map2.Visible = false;

            profileMap2Profile1Map1.Visible = false;
            profileMap2Profile1Map2.Visible = false;
            profileMap2Profile2Map1.Visible = false;
            profileMap2Profile2Map2.Visible = false;

            //Для профиля #1
            metroLabel214.Visible = false;
            metroLabel213.Visible = false;
            metroLabel247.Visible = false;
            metroLabel245.Visible = false;
            metroLabel249.Visible = false;
            metroLabel253.Visible = false;

            //Для профиля #2
            metroLabel265.Visible = false;
            metroLabel272.Visible = false;
            metroLabel258.Visible = false;
            metroLabel270.Visible = false;
            metroLabel243.Visible = false;
            metroLabel266.Visible = false;
        }

        #endregion

        #region Main
        // Проверка все ли профили загружены
        public void CheckOnLoadedLogins()
        {
            if(isLogin1Loaded & isLogin2Loaded)
            {
                CompareProfiles1.Visible = true;
            }
            else
            {
                CompareProfiles1.Visible = false;
            }
        }

        private void GetInfoMatch1_Click(object sender, EventArgs e)
        {
            metroPanel3.Visible = false;
            mLobby1.Items.Clear();
            mLobby2.Items.Clear();
            mLobby3.Items.Clear();
            mLobby4.Items.Clear();
            mLobby5.Items.Clear();
            mLobby6.Items.Clear();
            mLobby7.Items.Clear();
            mLobby8.Items.Clear();
            mLobby9.Items.Clear();
            mLobby10.Items.Clear();
            match = new Match();
            match.GetInfoMatch(matchIDbox1.Text);
            if((match.json.HasValues && match.json.Type != JTokenType.Null) && match.TeamA.leader != null)
            {
                isMatchLoaded = true;
                ShowInfoMatch();
                metroPanel3.Visible = true;
                metroTabControl1.SelectTab(2);
            }
        }

        private void getProfile1_Click_1(object sender, EventArgs e)
        {
            isLogin1Loaded = false;
            CheckOnLoadedLogins();
            metroPanel1.Visible = false;
            ClearProfileTab();
            profile = new Profile();
            profile.SetInfo(login1.Text);
            if (profile.json.HasValues & profile.json.Type != JTokenType.Null)
            {
                if (profile.dec.HasValues & profile.dec.Type != JTokenType.Null)
                {
                    
                    isLogin1Loaded = true;
                    ShowInfoProfileTab(profile);
                    CheckOnLoadedLogins();
                    DisableStatsCompareProfile();
                    loginProfileProfile1_defaultStyle();
                    loginMapBox1Profile1_defaultStyle();
                    loginMapBox2Profile1_defaultStyle();
                    metroTabControl1.SelectTab(0);
                    if(profile.friends.Count != 0)
                    {
                        friendsBoxProfile1.Enabled = true;
                        getFriendInfoProfile1.Enabled = true;
                        friendsBoxProfile1.SelectedIndex = 0;
                    }
                    else
                    {
                        friendsBoxProfile1.Enabled = false;
                        getFriendInfoProfile1.Enabled = false;
                    }
                    
                }
            }
        }

        private void Quit1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void goToFaceitProfile2_Click(object sender, EventArgs e)
        {
            Process.Start(profile2.faceit_url);
        }

        private void goToSteamProfile2_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + profile2.steamid64);
        }

        private void mNickname2_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamA.member2.faceit_profile.ToString());
        }

        private void mNickname3_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamA.member3.faceit_profile.ToString());
        }

        private void mNickname4_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamA.member4.faceit_profile.ToString());
        }

        private void mNickname5_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamA.member5.faceit_profile.ToString());
        }

        private void mNickname6_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamB.leader.faceit_profile.ToString());
        }

        private void mNickname7_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamB.member2.faceit_profile.ToString());
        }

        private void mNickname8_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamB.member3.faceit_profile.ToString());
        }

        private void mNickname9_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamB.member4.faceit_profile.ToString());
        }

        private void mNickname10_Click(object sender, EventArgs e)
        {
            Process.Start(match.TeamB.member5.faceit_profile.ToString());
        }

        #endregion

        private void CompareProfiles1_Click(object sender, EventArgs e)
        {
            SelectBestMapProfile1();
            SelectBestMapProfile2();
            int index1 = map1BoxProfile1.SelectedIndex;
            int index2 = map1BoxProfile2.SelectedIndex;
            Map map1 = profile.maps[index1];
            Map map2 = profile2.maps[index2];

            int indexMap1Profile1 = 0;
            int indexMap1Profile2 = 0;

            int indexMap2Profile1 = 0;
            int indexMap2Profile2 = 0;

            int indexProfile1 = 0;
            int indexProfile2 = 0;

            int indexAllProfile1 = 0;
            int indexAllProfile2 = 0;

            #region Map1
            if (map1.wins > map2.wins)
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.winRate > map2.winRate)
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.winRate < map2.winRate)
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kdratio > map2.kdratio)
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.kdratio < map2.kdratio)
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgHS > map2.avgHS)
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgHS < map2.avgHS)
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.death < map2.death)
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.death > map2.death)
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.krratio > map2.krratio)
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.krratio < map2.krratio)
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assist > map2.assist)
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.assist < map2.assist)
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.pentaKills > map2.pentaKills)
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.pentaKills < map2.pentaKills)
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadroKills > map2.quadroKills)
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.quadroKills < map2.quadroKills)
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.tripleKills > map2.tripleKills)
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.tripleKills < map2.tripleKills)
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.hsPerMatch > map2.hsPerMatch)
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.hsPerMatch < map2.hsPerMatch)
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKills > map2.avgKills)
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgKills < map2.avgKills)
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgDeath < map2.avgDeath)
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgDeath > map2.avgDeath)
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.totalHeadshots > map2.totalHeadshots)
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.totalHeadshots < map2.totalHeadshots)
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgAssist > map2.avgAssist)
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgAssist < map2.avgAssist)
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgMVPs > map2.avgMVPs)
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgMVPs < map2.avgMVPs)
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgPentaK > map2.avgPentaK)
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgPentaK < map2.avgPentaK)
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgQuadroK > map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgQuadroK < map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgTripleK > map2.avgTripleK)
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgTripleK < map2.avgTripleK)
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKRr > map2.avgKRr)
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgKRr < map2.avgKRr)
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKDr > map2.avgKDr)
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.avgKDr < map2.avgKDr)
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }
            #endregion

            map1 = new Map();
            map2 = new Map();
            index1 = map2BoxProfile1.SelectedIndex;
            index2 = map2BoxProfile2.SelectedIndex;
            map1 = profile.maps[index1];
            map2 = profile2.maps[index2];

            #region Map2
            if (map1.wins > map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.winRate > map2.winRate)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.winRate < map2.winRate)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kdratio > map2.kdratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.kdratio < map2.kdratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgHS > map2.avgHS)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgHS < map2.avgHS)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.death < map2.death)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.death > map2.death)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.krratio > map2.krratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.krratio < map2.krratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assist > map2.assist)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.assist < map2.assist)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.pentaKills > map2.pentaKills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.pentaKills < map2.pentaKills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadroKills > map2.quadroKills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.quadroKills < map2.quadroKills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.tripleKills > map2.tripleKills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.tripleKills < map2.tripleKills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.hsPerMatch > map2.hsPerMatch)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.hsPerMatch < map2.hsPerMatch)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKills > map2.avgKills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgKills < map2.avgKills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgDeath < map2.avgDeath)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgDeath > map2.avgDeath)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.totalHeadshots > map2.totalHeadshots)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.totalHeadshots < map2.totalHeadshots)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgAssist > map2.avgAssist)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgAssist < map2.avgAssist)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgMVPs > map2.avgMVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgMVPs < map2.avgMVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgPentaK > map2.avgPentaK)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgPentaK < map2.avgPentaK)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgQuadroK > map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgQuadroK < map2.avgQuadroK)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgTripleK > map2.avgTripleK)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgTripleK < map2.avgTripleK)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKRr > map2.avgKRr)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgKRr < map2.avgKRr)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.avgKDr > map2.avgKDr)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.avgKDr < map2.avgKDr)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            #endregion

            #region Profile
            if (profile.faceit_elo < profile2.faceit_elo)
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Red;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.faceit_elo > profile2.faceit_elo)
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Green;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.faceit_level < profile2.faceit_level)
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Red;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.faceit_level > profile2.faceit_level)
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Green;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.membership_type == "premium")
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else if (profile.membership_type == "supporter")
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (profile2.membership_type == "premium")
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else if (profile2.membership_type == "supporter")
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (profile.avgHS < profile2.avgHS)
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Red;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.avgHS > profile2.avgHS)
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Green;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.avgKD < profile2.avgKD)
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Red;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.avgKD > profile2.avgKD)
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Green;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.currrentWinStreak < profile2.currrentWinStreak)
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Red;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.currrentWinStreak > profile2.currrentWinStreak)
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Green;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.longestWinStreak < profile2.longestWinStreak)
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Red;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.longestWinStreak > profile2.longestWinStreak)
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Green;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.matches < profile2.matches)
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.matches > profile2.matches)
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.winMatches < profile2.winMatches)
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Red;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.winMatches > profile2.winMatches)
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Green;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.winRate < profile2.winRate)
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.winRate > profile2.winRate)
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.country == profile2.country)
            {
                countryProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                countryProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                countryProfile1.Style = MetroFramework.MetroColorStyle.Green;
                countryProfile2.Style = MetroFramework.MetroColorStyle.Green;
            }
            #endregion

            //Для первой карты двух профилей
            if (indexMap1Profile1 > indexMap1Profile2)
            {
                profileMap1Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if(indexMap1Profile1 < indexMap1Profile2)
            {
                profileMap1Profile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileMap1Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для второй карты двух профилей
            if (indexMap2Profile1 > indexMap2Profile2)
            {
                profileMap2Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap2Profile1 < indexMap2Profile2)
            {
                profileMap2Profile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileMap2Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            indexAllProfile1 = indexMap1Profile1 + indexMap2Profile1 + indexAllProfile1;
            indexAllProfile2 = indexMap1Profile2 + indexMap2Profile2 + indexAllProfile2;

            //Для всего профиля двух профилей
            if (indexAllProfile1 > indexAllProfile2)
            {
                profileAllProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexAllProfile1 < indexAllProfile2)
            {
                profileAllProfile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileAllProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для профиля двух профилей
            if (indexProfile1 > indexProfile2)
            {
                profileProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexProfile1 < indexProfile2)
            {
                profileProfile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            EnableStatsCompareProfile();
        }

        private void getFriendInfoProfile1_Click(object sender, EventArgs e)
        {
            login2.Text = friendsBoxProfile1.Items[friendsBoxProfile1.SelectedIndex].ToString();
            getProfile2.PerformClick();
        }

        private void getFriendInfoProfile2_Click(object sender, EventArgs e)
        {
            login1.Text = friendsBoxProfile2.Items[friendsBoxProfile2.SelectedIndex].ToString();
            getProfile1.PerformClick();
        }

        private void matchIDbox1_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = GetInfoMatch1;
        }

        private void login1_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = getProfile1;
        }

        private void login2_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = getProfile2;
        }

        private void settings1_Click(object sender, EventArgs e)
        {
            if (metroPanel2.Visible)
            {
                metroPanel2.Visible = false;
            }
            else
            {
                metroPanel2.Visible = true;
            }
        }

        private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (isLogin1Loaded)
            {
                map1BoxProfile1.Items.Clear();
                LoadMapInComboBox(map1BoxProfile1, profile.maps);
                map2BoxProfile1.Items.Clear();
                LoadMapInComboBox(map2BoxProfile1, profile.maps);
                loginMapBox1Profile1_defaultStyle();
                loginMapBox2Profile1_defaultStyle();
            }
            if (isLogin2Loaded)
            {
                map1BoxProfile2.Items.Clear();
                LoadMapInComboBox(map1BoxProfile2, profile2.maps);
                map2BoxProfile2.Items.Clear();
                LoadMapInComboBox(map2BoxProfile2, profile2.maps);
                loginMapBox1Profile2_defaultStyle();
                loginMapBox2Profile2_defaultStyle();
            }
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (isLogin1Loaded)
            {
                map1BoxProfile1.Items.Clear();
                LoadMapInComboBox(map1BoxProfile1, profile.maps);
                map2BoxProfile1.Items.Clear();
                LoadMapInComboBox(map2BoxProfile1, profile.maps);
                loginMapBox1Profile1_defaultStyle();
                loginMapBox2Profile1_defaultStyle();
            }
            if (isLogin2Loaded)
            {
                map1BoxProfile2.Items.Clear();
                LoadMapInComboBox(map1BoxProfile2, profile2.maps);
                map2BoxProfile2.Items.Clear();
                LoadMapInComboBox(map2BoxProfile2, profile2.maps);
                loginMapBox1Profile2_defaultStyle();
                loginMapBox2Profile2_defaultStyle();
            }
        }
    }

    public class ParserInteger
    {
        public static int ParsInt(string index)
        {
            return int.Parse(index.Replace(",", ""));
        }
        public static double ParsDouble(string index)
        {
            string temp = index.Replace(",", "");
            return double.Parse(temp.Replace(".",","));
        }
    }
    public class Map
    {
        public Image imgRegular;
        public Image imgSmall;
        public string mapName;
        public string mode;
        public string typeMap;
        public int assist;
        public double avgAssist;
        public double avgDeath;
        public double avgHS;
        public double avgKDr;
        public double avgKRr;
        public double avgKills;
        public double avgMVPs;
        public double avgTripleK;
        public double avgQuadroK;
        public double avgPentaK;
        public int death;
        public int hss;
        public double hsPerMatch;
        public double kdratio;
        public double krratio;
        public int kills;
        public int mvps;
        public int matches;
        public int pentaKills;
        public int quadroKills;
        public int tripleKills;
        public int rounds;
        public double totalHeadshots;
        public double winRate;
        public int wins;
    }
    public class Profile : Form1
    {
        public JObject json;
        public JObject dec = new JObject();
        public Image imgAvatar;
        public string faceit_url;
        public string player_id;
        public string nickname;
        public int faceit_elo;
        public int faceit_level;
        public string country;
        public string membership_type;
        public string steamid64;
        public List<string> friends_ids = new List<string>();
        public List<string> friends = new List<string>();
        public double avgHS;
        public double avgKD;
        public int currrentWinStreak;
        public int longestWinStreak;
        public int matches;
        public int winMatches;
        public double winRate;
        public List<Map> maps = new List<Map>();

        public void SetInfo(string login)
        {
            friends_ids = new List<string>();
            json = Form1.GetAPI(login);
            if (json.HasValues & json.Type != JTokenType.Null)
            {
                for (int i = 0; i < json["friends_ids"].ToList().Count; i++)
                {
                    friends_ids.Add(json["friends_ids"][i].ToString());
                }
                if (json["avatar"].Type == JTokenType.Null)
                {
                    imgAvatar = Form1.GetAvatar(null);
                }
                else
                {
                    imgAvatar = Form1.GetAvatar((string)json["avatar"]);
                }

                nickname = (string)json["nickname"];
                country = (string)json["country"];
                membership_type = (string)json["membership_type"];
                steamid64 = (string)json["steam_id_64"];
                player_id = (string)json["player_id"];
                faceit_url = json["faceit_url"].ToString().Replace("{lang}", "en");
                try
                {
                    faceit_level = ParserInteger.ParsInt(json["games"]["csgo"]["skill_level"].ToString());
                    faceit_elo = ParserInteger.ParsInt(json["games"]["csgo"]["faceit_elo"].ToString());
                }
                catch (Exception)
                {
                    Form2 form = new Form2();
                    form.errorMsg.Text = "Account not have csgo stats.";
                    form.Show();
                }
                SetStats(player_id);
                LoadFriends();
            }
        }

        public void SetStats(string playerID)
        {
            dec = Form1.GetStats(playerID);
            if (dec.HasValues & dec.Type != JTokenType.Null)
            {
                maps = new List<Map>();
                avgHS = ParserInteger.ParsDouble(dec["lifetime"]["Average Headshots %"].ToString());
                avgKD = ParserInteger.ParsDouble(dec["lifetime"]["Average K/D Ratio"].ToString());
                currrentWinStreak = ParserInteger.ParsInt(dec["lifetime"]["Current Win Streak"].ToString());
                longestWinStreak = ParserInteger.ParsInt(dec["lifetime"]["Longest Win Streak"].ToString());
                matches = ParserInteger.ParsInt(dec["lifetime"]["Matches"].ToString());
                winMatches = ParserInteger.ParsInt(dec["lifetime"]["Wins"].ToString());
                winRate = ParserInteger.ParsDouble(dec["lifetime"]["Win Rate %"].ToString());
                for (int i = 0; i < dec["segments"].ToList().Count; i++)
                {
                    Map map = new Map();
                    map.assist = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Assists"].ToString());
                    map.avgAssist = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Assists"].ToString());
                    map.avgDeath = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Deaths"].ToString());
                    map.avgHS = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Headshots %"].ToString());
                    map.avgKDr = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average K/D Ratio"].ToString());
                    map.avgKRr = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average K/R Ratio"].ToString());
                    map.avgKills = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Kills"].ToString());
                    map.avgMVPs = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average MVPs"].ToString());
                    map.avgPentaK = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Penta Kills"].ToString());
                    map.avgQuadroK = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Quadro Kills"].ToString());
                    map.avgTripleK = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Average Triple Kills"].ToString());
                    map.death = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Deaths"].ToString());
                    map.hsPerMatch = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Headshots per Match"].ToString());
                    map.hss = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Headshots"].ToString());
                    map.rounds = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Rounds"].ToString());
                    map.kdratio = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["K/D Ratio"].ToString());
                    map.kills = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Kills"].ToString());
                    map.krratio = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["K/R Ratio"].ToString());
                    map.mapName = (string)dec["segments"][i]["label"];
                    map.matches = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Matches"].ToString());
                    map.mode = (string)dec["segments"][i]["mode"];
                    map.mvps = ParserInteger.ParsInt(dec["segments"][i]["stats"]["MVPs"].ToString());
                    map.pentaKills = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Penta Kills"].ToString());
                    map.quadroKills = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Quadro Kills"].ToString());
                    map.tripleKills = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Triple Kills"].ToString());
                    map.typeMap = (string)dec["segments"][i]["type"];
                    map.winRate = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Win Rate %"].ToString());
                    map.totalHeadshots = ParserInteger.ParsDouble(dec["segments"][i]["stats"]["Total Headshots %"].ToString());
                    map.wins = ParserInteger.ParsInt(dec["segments"][i]["stats"]["Wins"].ToString());
                    maps.Add(map);
                }
                maps = maps.OrderBy(maps => maps.mapName).ToList();
            }
        }
        
        public void LoadFriends()
        {
            for(int i = 0; i < friends_ids.Count; i++)
            {
                try
                {
                    JObject res = Form1.GetProfile(friends_ids[i], false);
                    if ((string)res["nickname"] != null)
                    {
                        friends.Add((string)res["nickname"]);
                    }
                }
                catch (Exception)
                {
                    Form2 form = new Form2();
                    form.errorMsg.Text = "Friend not loaded. Friend №" + i;
                    form.Show();
                }
                
            }
        }
        
    }

    public class Match
    {
        public JObject json;
        public string serverCountry;
        public string region;
        public string type_match;
        public string map;
        public Uri demoURL;
        public int bestOf;
        public string winOf;
        public string scoreTeamA;
        public string scoreTeamB;
        public string status = null;
        public Uri matchURL;
        public Team1 TeamA = new Team1();
        public Team2 TeamB = new Team2();

        public void GetInfoMatch(string id)
        {
            json = Form1.GetMatch(id);
            if (json.HasValues && json.Type != JTokenType.Null)
            {
                region = json["region"].ToString();
                type_match = json["competition_type"].ToString();
                bestOf = ParserInteger.ParsInt(json["best_of"].ToString());
                winOf = json["results"]["winner"].ToString();
                scoreTeamA = json["results"]["score"]["faction1"].ToString();
                scoreTeamB = json["results"]["score"]["faction2"].ToString();
                status = json["status"].ToString();
                matchURL = Form1.SetUri(json["faceit_url"].ToString());
                try
                {
                    for (int i = 0; i < json["voting"].ToList().Count; i++)
                    {
                        serverCountry = json["voting"][i]["location"]["country"].ToString();
                        map = json["voting"][i]["map"]["name"].ToString();
                    }
                    for (int i = 0; i < json["voting"].ToList().Count; i++)
                    {
                        demoURL = new Uri(json["demo_url"][i].ToString());
                    }
                }
                catch (Exception)
                {
                    Form2 form = new Form2();
                    form.errorMsg.Text = @"Status of match voting\configuring\ongoing";
                    form.ShowDialog();
                }
                TeamA.SetTeam(json);
                TeamB.SetTeam(json);
            }
            
        }
    }
    public class Team1
    {
        public Member leader;
        public Member member2;
        public Member member3;
        public Member member4;
        public Member member5;
        public string leaderID;
        public string teamName;
        public string type;
        public string fraction = "faction1";

        public void SetTeam(JObject dec)
        {
            leaderID = dec["teams"]["faction1"]["leader"].ToString();
            List<Member> memb = new List<Member>();
            Member mem;
            try
            {
                for (int i = 0; i < dec["teams"]["faction1"]["roster_v1"].ToList().Count; i++)
                {
                    mem = new Member();
                    mem.join_type = dec["teams"]["faction1"]["roster_v1"][i]["quick_match"]["join_type"].ToString();
                    mem.nickname = dec["teams"]["faction1"]["roster_v1"][i]["nickname"].ToString();
                    mem.csgoID = dec["teams"]["faction1"]["roster_v1"][i]["csgo_id"].ToString();
                    mem.guid = dec["teams"]["faction1"]["roster_v1"][i]["guid"].ToString();
                    for (int k = 0; k < dec["teams"]["faction1"]["roster_v1"][i]["quick_match"]["selected_members_ids"].ToList().Count; k++)
                    {
                        mem.select_members_id.Add(dec["teams"]["faction1"]["roster_v1"][i]["quick_match"]["selected_members_ids"][k].ToString());
                    }
                    memb.Add(mem);
                }
                for (int i = 0; i < memb.Count; i++)
                {
                    if (memb[i].guid == leaderID)
                    {
                        leader = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member2 == null)
                    {
                        member2 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member3 == null)
                    {
                        member3 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member4 == null)
                    {
                        member4 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member5 == null)
                    {
                        member5 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                }
                teamName = dec["teams"]["faction1"]["name"].ToString();
                type = dec["teams"]["faction1"]["type"].ToString();
            }
            catch (Exception)
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "CS: GO stats not found.";
                form.errorMsg.Show();
            }
        }
    }
    public class Team2
    {
        public Member leader;
        public Member member2;
        public Member member3;
        public Member member4;
        public Member member5;
        public string leaderID;
        public string teamName;
        public string type;
        public string fraction = "faction2";

        public void SetTeam(JObject dec)
        {
            leaderID = dec["teams"]["faction2"]["leader"].ToString();
            List<Member> memb = new List<Member>();
            Member mem;
            try
            {
                for (int i = 0; i < dec["teams"]["faction2"]["roster_v1"].ToList().Count; i++)
                {
                    mem = new Member();
                    mem.join_type = dec["teams"]["faction2"]["roster_v1"][i]["quick_match"]["join_type"].ToString();
                    mem.nickname = dec["teams"]["faction2"]["roster_v1"][i]["nickname"].ToString();
                    mem.csgoID = dec["teams"]["faction2"]["roster_v1"][i]["csgo_id"].ToString();
                    mem.guid = dec["teams"]["faction2"]["roster_v1"][i]["guid"].ToString();
                    for (int k = 0; k < dec["teams"]["faction2"]["roster_v1"][i]["quick_match"]["selected_members_ids"].ToList().Count; k++)
                    {
                        mem.select_members_id.Add(dec["teams"]["faction2"]["roster_v1"][i]["quick_match"]["selected_members_ids"][k].ToString());
                    }
                    memb.Add(mem);
                }
                for (int i = 0; i < memb.Count; i++)
                {
                    if (memb[i].guid == leaderID)
                    {
                        leader = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member2 == null)
                    {
                        member2 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member3 == null)
                    {
                        member3 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member4 == null)
                    {
                        member4 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                    else if (member5 == null)
                    {
                        member5 = memb[i];
                        memb[i].GetInfo();
                        memb[i].GetRanking(dec["region"].ToString());
                    }
                }
                teamName = dec["teams"]["faction2"]["name"].ToString();
                type = dec["teams"]["faction2"]["type"].ToString();
            }
            catch (Exception)
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "CS: GO stats not found.";
                form.errorMsg.Show();
            }
        }
    }
    public class Member
    {
        public string country;
        public Image avatar;
        public int faceit_level;
        public string membership_type;
        public string join_type;
        public string nickname;
        public string csgoID;
        public int wins;
        public double matches;
        public double winRate;
        public int faceit_elo;
        public double avgHS;
        public double avgKDr;
        public int ranking;
        public Uri faceit_profile;
        public List<string> select_members_id = new List<string>();
        public string guid;

        public void GetInfo()
        {
            try
            {
                JObject dec = Form1.GetAPI(nickname);
                country = dec["country"].ToString();
                faceit_level = ParserInteger.ParsInt(dec["games"]["csgo"]["skill_level"].ToString());
                faceit_elo = ParserInteger.ParsInt(dec["games"]["csgo"]["faceit_elo"].ToString());
                membership_type = dec["membership_type"].ToString();
                faceit_profile = Form1.SetUri(dec["faceit_url"].ToString());
                guid = dec["player_id"].ToString();
                if (dec["avatar"].Type == JTokenType.Null)
                {
                    avatar = Form1.GetAvatar(null);
                }
                else
                {
                    avatar = Form1.GetAvatar((string)dec["avatar"]);
                }
            }
            catch (Exception)
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Member not loaded.";
                form.ShowDialog();
            }
            try
            {
                var dec1 = JObject.Parse(Form1.GetProfile(guid));
                avgHS = ParserInteger.ParsDouble(dec1["lifetime"]["Average Headshots %"].ToString());
                avgKDr = ParserInteger.ParsDouble(dec1["lifetime"]["Average K/D Ratio"].ToString());
                matches = ParserInteger.ParsDouble(dec1["lifetime"]["Matches"].ToString());
                wins = ParserInteger.ParsInt(dec1["lifetime"]["Wins"].ToString());
                winRate = ParserInteger.ParsDouble(dec1["lifetime"]["Win Rate %"].ToString());
            }
            catch (Exception)
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Member not loaded. Method 'GetInfo2'.";
                form.ShowDialog();
            }
            
        }
        public void GetRanking(string reg)
        {
            var dec2 = JObject.Parse(Form1.GetRanking(reg, guid));
            ranking = ParserInteger.ParsInt(dec2["position"].ToString());
        }
    }
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) || 
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
    public static class ConnectivityChecker
    {
        public enum ConnectionStatus
        {
            NotConnected,
            LimitedAccess,
            Connected
        }

        public static ConnectionStatus CheckInternet()
        {
            // Проверить подключение к dns.msftncsi.com
            try
            {
                IPHostEntry entry = Dns.GetHostEntry("dns.msftncsi.com");
                if (entry.AddressList.Length == 0)
                {
                    return ConnectionStatus.NotConnected;
                }
                else
                {
                    if (!entry.AddressList[0].ToString().Equals("131.107.255.255"))
                    {
                        return ConnectionStatus.LimitedAccess;
                    }
                }
            }
            catch
            {
                return ConnectionStatus.NotConnected;
            }

            // Проверить загрузку документа ncsi.txt
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.msftncsi.com/ncsi.txt");
            try
            {
                HttpWebResponse responce = (HttpWebResponse)request.GetResponse();

                if (responce.StatusCode != HttpStatusCode.OK)
                {
                    return ConnectionStatus.LimitedAccess;
                }
                using (StreamReader sr = new StreamReader(responce.GetResponseStream()))
                {
                    if (sr.ReadToEnd().Equals("Microsoft NCSI"))
                    {
                        return ConnectionStatus.Connected;
                    }
                    else
                    {
                        return ConnectionStatus.LimitedAccess;
                    }
                }
            }
            catch
            {
                return ConnectionStatus.NotConnected;
            }

        }
    }
}
