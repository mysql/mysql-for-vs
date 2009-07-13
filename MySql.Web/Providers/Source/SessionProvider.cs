using System;
using System.Web;
using System.Web.SessionState;

namespace MySql.Web.SessionState
{
    class SessionProvider : SessionStateStoreProviderBase
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            throw new NotImplementedException();
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void EndRequest(System.Web.HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            throw new NotImplementedException();
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            throw new NotImplementedException();
        }

        public override void InitializeRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            throw new NotImplementedException();
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            throw new NotImplementedException();
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            throw new NotImplementedException();
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            throw new NotImplementedException();
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            throw new NotImplementedException();
        }
    }
}
