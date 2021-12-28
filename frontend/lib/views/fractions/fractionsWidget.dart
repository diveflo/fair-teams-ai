import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/state/appState.dart';
import 'package:no_cry_babies/state/gameState.dart';
import 'package:no_cry_babies/views/fractions/largeFractionsWidget.dart';
import 'package:no_cry_babies/views/fractions/smallFractionsWidget.dart';

class FractionsWidget extends StatelessWidget {
  const FractionsWidget({
    Key key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, GameState>(
      converter: (store) => store.state.gameState,
      builder: (context, game) {
        return LayoutBuilder(
            builder: (BuildContext context, BoxConstraints constraints) {
          if (constraints.maxWidth > 600) {
            return LargeFractionsWidget();
          }
          return SmallFractionsWidget();
        });
      },
    );
  }
}
