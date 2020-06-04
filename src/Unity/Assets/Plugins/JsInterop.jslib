mergeInto(LibraryManager.library, {
  PropagateValue: function (id, value) {
    window["UnityInterop"].deliverThrowValue(Pointer_stringify(id), value);
  }
});