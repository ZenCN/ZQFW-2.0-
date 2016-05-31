window.plugins = window.plugins || {};

var cSpeed = 3;
// var cWidth = 124;
//var cHeight = 128;
var cTotalFrames = 20;
var cFrameWidth = 124;
var cImageSrc = '../../../CSS/Img/Public/Sprites.png';

var cImageTimeout = false;
var cIndex = 0;
var cXpos = 0;
var cPreloaderTimeout = false;
var SECONDS_BETWEEN_FRAMES = 0;

window.plugins.preloader = function (state) {
    function startAnimation() {

        //document.getElementById('loaderImage').style.backgroundImage = 'url(' + cImageSrc + ')';
        //document.getElementById('loaderImage').style.width = cWidth + 'px';
        //document.getElementById('loaderImage').style.height = cHeight + 'px';

        //FPS = Math.round(100/(maxSpeed+2-speed));
        FPS = Math.round(100 / cSpeed);
        SECONDS_BETWEEN_FRAMES = 1 / FPS;

        cPreloaderTimeout = setTimeout(continueAnimation, SECONDS_BETWEEN_FRAMES / 1000);

    }

    function continueAnimation() {

        cXpos += cFrameWidth;
        //increase the index so we know which frame of our animation we are currently on
        cIndex += 1;

        //if our cIndex is higher than our total number of frames, we're at the end and should restart
        if (cIndex >= cTotalFrames) {
            cXpos = 0;
            cIndex = 0;
        }

        if (document.getElementById('loaderImage'))
            document.getElementById('loaderImage').style.backgroundPosition = (-cXpos) + 'px 0';

        cPreloaderTimeout = setTimeout(continueAnimation, SECONDS_BETWEEN_FRAMES * 1000);
    }

    function stopAnimation() { //stops animation
        clearTimeout(cPreloaderTimeout);
        cPreloaderTimeout = false;
    }

    function imageLoader(s, fun)//Pre-loads the sprites image
    {
        clearTimeout(cImageTimeout);
        cImageTimeout = 0;
        genImage = new Image();
        genImage.onload = function() { cImageTimeout = setTimeout(fun, 0) };
        genImage.onerror = new Function('alert(\'Could not load the image\')');
        genImage.src = s;
    }

    //The following code starts the animation
    if (state == 'start' && window.plugins.preloader.started) {
        //state = 'continue';
        return false;
    }

    switch (state) {
        case 'start':
            $('body #preloader').show();
            new imageLoader(cImageSrc, startAnimation);
            window.plugins.preloader.started = true;
            break;
        case 'continue':
            $('body #preloader').show();
            new imageLoader(cImageSrc, continueAnimation);
            break;
        case 'stop':
            new imageLoader(cImageSrc, stopAnimation);
            $('body #preloader').fadeOut('slow');
            window.plugins.preloader.started = false;
            break;
    }
};

window.plugins.preloader.started = false;

$(function() {
    window.plugins.preloader('start');
});