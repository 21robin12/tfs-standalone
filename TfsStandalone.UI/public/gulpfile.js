// load modules
var gulp = require('gulp');
var sass = require('gulp-sass');
var concat = require('gulp-concat');
var include = require('./include.json');

function swallowError(error) {
    console.log(error.toString());
    this.emit('end');
}

gulp.task('styles', function () {
    gulp.src('scss/**/global.scss')
        .pipe(sass({
            errLogToConsole: true
        }))
        .on('error', swallowError)
        .pipe(gulp.dest('./dist/css'));
});

gulp.task('scripts', function() {
    return gulp.src(include.files)
        .pipe(concat('global.js'))
        .on('error', swallowError)
        .pipe(gulp.dest('./dist/js'));
});

gulp.task('watch', function () {
    gulp.watch('scss/**/*.scss', ['styles']);
    gulp.watch('js/**/*.js', ['scripts']);
});

gulp.task('default', function () {
    // can pass many tasks here as an array
    return gulp.start(['styles', 'scripts', 'watch']);
});