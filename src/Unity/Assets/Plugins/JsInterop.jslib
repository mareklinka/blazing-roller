mergeInto(LibraryManager.library, {
  PropagateValue: function (id, value, config) {
    window["UnityInterop"].deliverThrowValue(Pointer_stringify(id), value, Pointer_stringify(config));
  }
});