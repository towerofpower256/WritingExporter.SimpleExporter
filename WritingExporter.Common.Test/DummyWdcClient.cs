using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using System.IO;
using System.Threading;

namespace WritingExporter.Common.Test
{
    public enum DummyWdcClientMode
    {
        LoggedOut,
        LoggedIn,
        LoggedInPaid,
        LoggedInStoryRestricted,
        InteractivesUnavailableLoggedOut,
        InteractivesUnavailableLoggedIn,
    }

    public class DummyWdcClient : BaseWdcClient, IWdcClient
    {
        //Dictionary<string, string> _payloadCache;
        string _dataDir;
        //object _lock;
        DummyWdcClientMode _mode;

        public DummyWdcClient(string dataDir, DummyWdcClientMode mode = DummyWdcClientMode.LoggedIn)
        {
            this._dataDir = dataDir;
            //this._payloadCache = new Dictionary<string, string>();
            this._mode = mode;
        }

        public async Task<WdcResponse> GetInteractiveChapter(string interactiveID, string chapterID, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<WdcResponse> GetInteractiveHomepage(string interactiveID, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<WdcResponse> GetInteractiveOutline(string interactiveID, CancellationToken ct)
        {
            var r = new WdcResponse();
            r.Address = interactiveID;
            r.WebResponse = DoInteractivesUnavailable();

            if (string.IsNullOrEmpty(r.WebResponse))
            {
                if (_mode == DummyWdcClientMode.LoggedIn || _mode == DummyWdcClientMode.LoggedInPaid)
                {
                    r.WebResponse = GetPage("Looking for adventure - outline - logged in.html");
                }
                else
                {
                    r.WebResponse = GetPage("Looking for adventure - outline - logged out.html");
                }
            }

            return r;
        }

        public async Task<WdcResponse> GetInteractiveRecentAdditions(string interactiveID, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToInteractive(string storyId)
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToInteractiveChapter(string storyId, string chapterId)
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToInteractiveOutline(string storyId)
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToInteractiveRecentAdditions(string storyId)
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToLogin()
        {
            throw new NotImplementedException();
        }

        public Uri GetPathToRoot()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            //lock (_lock)
            //{
            //    this._payloadCache.Clear();
            //}
        }

        private string DoInteractivesUnavailable()
        {
            if (_mode == DummyWdcClientMode.InteractivesUnavailableLoggedIn)
            {
                return GetPage("Interactives unavailable - logged in.html");
            }
            else if (_mode == DummyWdcClientMode.InteractivesUnavailableLoggedOut)
            {
                return GetPage("Interactives unavailable - logged out.html");
            }
            else
            {
                return null;
            }
        }

        private string GetPage(string fname)
        {
            //lock (_lock)
            //{
            //    var fPath = Path.Combine(_dataDir, fname);
            //    // If we've seent his file before, just return it
            //    if (_payloadCache.ContainsKey(fPath)) return _payloadCache[fPath];

            //    return File.ReadAllText(fPath);
            //}

            return TestUtil.GetDataFile(Path.Combine(_dataDir, fname));
        }
    }
}
