import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/largeAppLayoutWidget.dart';
import 'package:no_cry_babies/views/smallAppLayoutWidget.dart';

const int displayThreshold = 600;

class AppLayoutWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
        builder: (BuildContext context, BoxConstraints constraints) {
      if (constraints.maxWidth > displayThreshold) {
        return LargeAppLayoutWidget();
      }
      return SmallAppLayoutWidget();
    });
  }
}
