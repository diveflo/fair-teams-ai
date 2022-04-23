import 'package:flutter/material.dart';
import 'package:no_cry_babies/views/botsWidget.dart';
import 'package:no_cry_babies/views/candidates/candidatesWidget.dart';
import 'package:no_cry_babies/views/fractions/fractionsWidget.dart';
import 'package:no_cry_babies/views/mapPool/mapPoolWidget.dart';
import 'package:no_cry_babies/views/newPlayerWidget.dart';
import 'package:no_cry_babies/views/scrambleWidget.dart';

/// This class creates the app layout for large screen sizes
class LargeAppLayoutWidget extends StatelessWidget {
  final title;

  LargeAppLayoutWidget(
    this.title, {
    Key key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Theme.of(context).highlightColor,
        title: Text(title),
        leading: Image(image: AssetImage("assets/hnyb.jpg")),
      ),
      body: Center(
          // Center is a layout widget. It takes a single child and positions it
          // in the middle of the parent.
          child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            // Upper layout row
            Expanded(
              flex: 2,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  Expanded(
                    flex: 5,
                    child: Image(
                        image: AssetImage("assets/cs.jpg"),
                        fit: BoxFit.fitHeight),
                  ),
                  Expanded(
                    flex: 4,
                    child: CandidatesWidget(),
                  ),
                  Expanded(
                    flex: 4,
                    child: Column(children: [
                      NewPlayerWidget(),
                      SizedBox(
                        height: 30,
                      ),
                      BotsWidget(),
                      ScrambleWidget(),
                    ]),
                  )
                ],
              ),
            ),
            SizedBox(
              height: 30,
            ),
            // lower layout row
            Expanded(
              flex: 2,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  Expanded(
                    flex: 2,
                    child: FractionsWidget(),
                  ),
                  Expanded(
                    flex: 1,
                    child: MapPoolWidget(),
                  )
                ],
              ),
            ),
          ],
        ),
      )),
    );
  }
}
