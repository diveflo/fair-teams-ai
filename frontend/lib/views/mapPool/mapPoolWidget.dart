import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/mapPool/mapPoolHeaderWidget.dart';
import 'package:no_cry_babies/views/mapPool/mapPoolListWidget.dart';

class MapPoolWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        MapPoolHeaderWidget(),
        Expanded(
          child: MapPoolListWidget(),
        ),
      ],
    );
  }
}
