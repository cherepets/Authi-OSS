Object.defineProperty(Object.prototype, 'toJson', {
    value: function () {
        return JSON.stringify(this);
    },
    enumerable: false
});

Object.defineProperty(String.prototype, 'fromJson', {
    value: function () {
        return JSON.parse(this);
    },
    enumerable: false
});