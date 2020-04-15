# Change Log

## 1.1.8 (2017-06-12)

**Features:**

  - Change back to .mt2 and add mirror path (.LOG)
  - Can show pallet folder by shift+click on folder name.
  - Change circle defect to red color.

## 1.1.7 (2017-06-06)

**Fixed:**

  - Change .mt2 to be .LOG

## 1.1.6 (2017-05-24)

**Features:**

  - Support custom image file type ex. bmp or jpg.

**Fixed:**

  - Fix searchPallet when HGA S/N is blank.
  - Disable confirm tray when run as pallet.

## 1.1.4 (2017-04-25)

**Features:**

  - Add new aqtray path for .mt2 file and backup file
  - Log S/N when updating

## 1.1.3 (2017-04-24)

**Fixed bugs:**

  - fix bug confirmMultipleGood with updating vmiResult

## ------- Previous Generation -------

## 1.0.3 beta (2015-09-30)

**Features:**

  - Change format of result file in test mode (GR&R), add image path at the first line, not compatible with older version
  - Add time count in result file.

**Implemented enhancements:**

  - Add unit testing for HGAItemTest, ImageWithDefectTest
  - Refactor code

**Fixed bugs:**

  - Position of ground pad in J4P template.
  - Ensure XML file is closed after reading.

## 1.0.2 beta (2015-09-23)

**Features:**

  - Add random picture in test mode (GR&R)
  
**Fixed bugs:**

  - Fix hang when overwrite opening result file by creating new unique file.

## 1.0.1 beta (2015-09-16)

**Implemented enhancements:**

  - Add global exception handler
  - Read MaxOrder & startPictureId from result file (csv) in Playback mode.

**Fixed bugs:**

  - Add file used by another process handler
  - Fix background worker is busy
  - Fix image path when loading from result file (csv) 
