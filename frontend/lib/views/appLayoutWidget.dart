import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/largeAppLayoutWidget.dart';
import 'package:no_cry_babies/views/smallAppLayoutWidget.dart';

class AppLayoutWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
        builder: (BuildContext context, BoxConstraints constraints) {
      if (constraints.maxWidth > 600) {
        return LargeAppLayoutWidget();
      }
      return SmallAppLayoutWidget();
    });
  }
}
