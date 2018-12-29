const gulp = require("gulp");
const sass = require("gulp-sass");
const concat = require("gulp-concat");
const cleanCSS = require("gulp-clean-css");
const terser = require("gulp-terser");

const css = [
    "wwwroot/css/site.css"
];

const js = [
    "node_modules/jquery/dist/jquery.js",
    "node_modules/jquery-validation/dist/jquery.validate.js",
    "node_modules/jquery-validation/dist/additional-methods.js",
    "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
    "node_modules/popper.js/dist/umd/popper.js",
    "node_modules/bootstrap/dist/js/bootstrap.js",
    "node_modules/bootstrap4-notify/bootstrap-notify.js",
    "wwwroot/js/site.js"
];

gulp.task("copy-font-awesome-fonts", function () {
    return gulp.src("node_modules/@fortawesome/fontawesome-free/webfonts/fa-solid-*")
        .pipe(gulp.dest("wwwroot/fonts"));
});

gulp.task("build-scss", function () {
    return gulp.src("scss/site.scss")
        .pipe(sass())
        .pipe(gulp.dest("wwwroot/css"));
});

gulp.task("build-js", function() {
    return gulp.src(js)
        .pipe(terser())
        .pipe(concat('site.min.js'))
        .pipe(gulp.dest('wwwroot/js'));
});

gulp.task("build-css", gulp.series("copy-font-awesome-fonts", "build-scss", function() {
    return gulp.src(css)
        .pipe(cleanCSS({level: {1: {specialComments: 0}}}))
        .pipe(concat('site.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
}));

gulp.task("build-assets", gulp.parallel("build-css", "build-js"));

gulp.task("default", gulp.series("build-assets"));