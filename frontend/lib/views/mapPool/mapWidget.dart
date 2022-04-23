import 'package:flutter/material.dart';

class MapWidget extends StatelessWidget {
  final mapCallsPath;

  const MapWidget(this.mapCallsPath, {Key key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: InteractiveViewer(
        child: Image(
          image: AssetImage(mapCallsPath),
        ),
      ),
    );
  }
}
