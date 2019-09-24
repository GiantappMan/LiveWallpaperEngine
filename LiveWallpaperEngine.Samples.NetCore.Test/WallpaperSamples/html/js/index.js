'use strict';

var timeElm = document.getElementById('time');
var doc = document.documentElement;
var clientWidth = doc.clientWidth;
var clientHeight = doc.clientHeight;

var pad = function pad(val) {
  return val < 10 ? '0' + val : val;
};

var time$ = Rx.Observable.interval(1000).map(function () {
  var time = new Date();

  return {
    hours: time.getHours(),
    minutes: time.getMinutes(),
    seconds: time.getSeconds()
  };
}).subscribe(function (_ref) {
  var hours = _ref.hours;
  var minutes = _ref.minutes;
  var seconds = _ref.seconds;

  timeElm.setAttribute('data-hours', pad(hours));
  timeElm.setAttribute('data-minutes', pad(minutes));
  timeElm.setAttribute('data-seconds', pad(seconds));
});

var mouse$ = Rx.Observable.fromEvent(document, 'mousemove').map(function (_ref2) {
  var clientX = _ref2.clientX;
  var clientY = _ref2.clientY;
  return {
    x: (clientWidth / 2 - clientX) / clientWidth,
    y: (clientHeight / 2 - clientY) / clientHeight
  };
});

RxCSS({
  mouse: RxCSS.animationFrame.withLatestFrom(mouse$, function (_, m) {
    return m;
  }).scan(RxCSS.lerp(0.2))
}, timeElm);