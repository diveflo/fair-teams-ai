import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/mapPool/mapPoolListWidget.dart';
import 'package:no_cry_babies/views/maps/mapPoolHeaderWidget.dart';

class MapPoolWidget extends StatefulWidget {
  @override
  _MapPoolWidgetState createState() => _MapPoolWidgetState();
}

class _MapPoolWidgetState extends State<MapPoolWidget> {
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
