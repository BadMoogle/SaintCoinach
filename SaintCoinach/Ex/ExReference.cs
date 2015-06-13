﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintCoinach.Ex {
    public sealed class ExReference<T> where T : class {
        #region Fields
        private readonly WeakReference<T> _WeakRef;
        private T _StrongRef;
        private readonly ExCollection _Collection;
        #endregion

        #region Constructor
        public ExReference(ExCollection collection, T file) {
            _WeakRef = new WeakReference<T>(file);
            _Collection = collection;
            if (collection._Optimize)
                _StrongRef = file;
        }
        #endregion

        #region TryGet
        public bool TryGetTarget(out T target) {
            var useStrong = _Collection._Optimize;

            if (_StrongRef != null) {
                target = _StrongRef;
                if (!useStrong)
                    _StrongRef = null;

                return true;
            }

            var result = _WeakRef.TryGetTarget(out target);
            if (result && useStrong)
                _StrongRef = target;
            return result;
        }
        public void SetTarget(T target) {
            _WeakRef.SetTarget(target);
            if (_Collection._Optimize)
                _StrongRef = target;
            else
                _StrongRef = null;
        }
        #endregion
    }
}
