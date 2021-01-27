# cs-image-bundle-for-print
A c# console program for bundle images on a singel A4 contact sheet.


# How to use
Drop selected images to exe and it will mount them on a A4 image size at 96dpi ready for print.
It will scale down images if needed to fit there given space on the contact sheet.

# Options
Change name on exe file to add options

***-v*** Verbose


***-dpi*** Dpi settings between 20 and 200


***-s*** Size settings, A3, A4, A5


```

ImageBundleForPrint -v -s a5 -dpi 200.exe

//will be verbose, paper size a5, and output dpi 200
```

# Output examples
![example output](example_output.jpg)
