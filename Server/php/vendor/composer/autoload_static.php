<?php

// autoload_static.php @generated by Composer

namespace Composer\Autoload;

class ComposerStaticInit4e78b9fa4a5f57d9983d4961cb2ebf4f
{
    public static $prefixLengthsPsr4 = array (
        'T' => 
        array (
            'Tests\\' => 6,
        ),
        'R' => 
        array (
            'ReallySimpleJWT\\' => 16,
        ),
    );

    public static $prefixDirsPsr4 = array (
        'Tests\\' => 
        array (
            0 => __DIR__ . '/..' . '/rbdwllr/reallysimplejwt/tests',
        ),
        'ReallySimpleJWT\\' => 
        array (
            0 => __DIR__ . '/..' . '/rbdwllr/reallysimplejwt/src',
        ),
    );

    public static $classMap = array (
        'Composer\\InstalledVersions' => __DIR__ . '/..' . '/composer/InstalledVersions.php',
    );

    public static function getInitializer(ClassLoader $loader)
    {
        return \Closure::bind(function () use ($loader) {
            $loader->prefixLengthsPsr4 = ComposerStaticInit4e78b9fa4a5f57d9983d4961cb2ebf4f::$prefixLengthsPsr4;
            $loader->prefixDirsPsr4 = ComposerStaticInit4e78b9fa4a5f57d9983d4961cb2ebf4f::$prefixDirsPsr4;
            $loader->classMap = ComposerStaticInit4e78b9fa4a5f57d9983d4961cb2ebf4f::$classMap;

        }, null, ClassLoader::class);
    }
}
