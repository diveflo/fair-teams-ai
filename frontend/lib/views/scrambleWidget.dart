import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/thunks/scramble.dart';
import 'package:frontend/views/myButtonWidget.dart';

class ScrambleWidget extends StatefulWidget {
  const ScrambleWidget({
    Key key,
  }) : super(key: key);

  @override
  _ScrambleWidgetState createState() => _ScrambleWidgetState();
}

class _ScrambleWidgetState extends State<ScrambleWidget> {
  void _scramblePlayers() {
    StoreProvider.of<AppState>(context).dispatch(scrambleTeamsRandom());
  }

  void _scrambleApi() {
    StoreProvider.of<AppState>(context).dispatch(scrambleTeams());
  }

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, bool>(
      converter: (store) => store.state.gameState.isLoading,
      builder: (context, isLoading) {
        return Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 10),
              child: MyButtonWidget(
                buttonText: "Scramble",
                onPressed: _scramblePlayers,
                color: Colors.lime,
              ),
            ),
            isLoading ? CircularProgressIndicator() : Container(),
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 10),
              child: MyButtonWidget(
                buttonText: "ScrambleApi",
                onPressed: _scrambleApi,
                color: Colors.lime,
              ),
            ),
          ],
        );
      },
    );
  }
}
