import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/map.dart';
import 'package:frontend/state/appState.dart';

class MapPoolWidget extends StatefulWidget {
  @override
  _MapPoolWidgetState createState() => _MapPoolWidgetState();
}

class _MapPoolWidgetState extends State<MapPoolWidget> {
  MapPool mapPool;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          "Maps",
          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 30),
        ),
        Container(
          child: Expanded(
            child: StoreConnector<AppState, MapPool>(
              converter: (store) => store.state.gameConfigState.mapPool,
              builder: (context, mapPool) {
                return ListView.builder(
                  itemCount: mapPool.maps.length,
                  itemBuilder: (BuildContext context, int index) {
                    return Card(
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                          side: BorderSide(
                              color: mapPool.maps[index].isDismissed
                                  ? Colors.grey
                                  : Colors.green,
                              width: 2),
                        ),
                        child: CheckboxListTile(
                          value: mapPool.maps[index].isDismissed,
                          onChanged: (bool value) {
                            setState(() {
                              mapPool.maps[index].isDismissed =
                                  !mapPool.maps[index].isDismissed;
                            });
                          },
                          secondary: Image.asset(
                            mapPool.maps[index].imagePath,
                          ),
                          title: Text(mapPool.maps[index].name),
                        ));
                  },
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}
