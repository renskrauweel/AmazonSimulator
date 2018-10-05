    function placeBaggageHall(scene) {
        //baggage hall
        var geometry = new THREE.PlaneGeometry(30, 30, 32);
        var material = new THREE.MeshPhongMaterial({map: new THREE.TextureLoader().load("textures/floor.jpg"), side: THREE.DoubleSide });
        var plane = new THREE.Mesh(geometry, material);
        plane.rotation.x = Math.PI / 2.0;
        plane.position.x = 15;
        plane.position.z = 15;
        scene.add(plane);
    }
    function placeRunway(scene) {
        //Runway
        var geometry = new THREE.PlaneGeometry(150, 30);
        var material = new THREE.MeshPhongMaterial({color: 0x3a3a3a, side: THREE.DoubleSide });
        var runway = new THREE.Mesh(geometry, material);
        runway.rotation.x = Math.PI / 2.0;
        runway.position.x = 15;
        runway.position.z = -15;
        scene.add(runway);

        //Runway white stripes
        var geometry = new THREE.PlaneGeometry(4, 1.5);
        var material = new THREE.MeshPhongMaterial({color: 0xffffff, side: THREE.DoubleSide });
        for (var i = 0; i < 17; i++) {
            var x = -56 + i * 9;

            var stripe = new THREE.Mesh(geometry, material);
            stripe.rotation.x = Math.PI / 2.0;
            stripe.position.x = x;
            stripe.position.z = -15;
            stripe.position.y = 0.001;
            scene.add(stripe);

        }
    }
    function placeRoads(scene) {
        // Roads
        var geometry = new THREE.PlaneGeometry(20, 1);
        var material = new THREE.MeshPhongMaterial({color: 0x6f6f6f, side: THREE.DoubleSide });
        var road = new THREE.Mesh(geometry, material);
        road.rotation.x = Math.PI / 2.0;
        road.position.x = 15;
        road.position.z = 5;
        road.position.y = .01;
        scene.add(road);
        // Lanes
        for (var i = 0; i < 4; i++) {
            var z = 10 + i * 5;
            var lane = new THREE.Mesh(geometry, material);
            lane.rotation.x = Math.PI / 2.0;
            lane.position.x = 15;
            lane.position.z = z;
            lane.position.y = .01;
            scene.add(lane);
        }

        // Sides
        var geometry = new THREE.PlaneGeometry(1, 21);
        var sideLeft = new THREE.Mesh(geometry, material);
        sideLeft.rotation.x = Math.PI / 2.0;
        sideLeft.position.x = 5;
        sideLeft.position.z = 15;
        sideLeft.position.y = .01;
        scene.add(sideLeft);
        var sideRight = new THREE.Mesh(geometry, material);
        sideRight.rotation.x = Math.PI / 2.0;
        sideRight.position.x = 25;
        sideRight.position.z = 15;
        sideRight.position.y = .01;
        scene.add(sideRight);
    }
    function placeSkybox(scene) {
        var sphericalSkyboxGeometry = new THREE.SphereGeometry(500, 32, 32);
        var sphericalSkyboxMaterial = new THREE.MeshPhongMaterial({map: new THREE.TextureLoader().load("textures/Street View 360.jpg"), side: THREE.DoubleSide });
        var sphericalSkybox = new THREE.Mesh(sphericalSkyboxGeometry, sphericalSkyboxMaterial);
        scene.add(sphericalSkybox);
    }
    function placeGarage(scene) {
        //// Instantiate a loader
        //var loader = new THREE.GLTFLoader();

        //// Optional: Provide a DRACOLoader instance to decode compressed mesh data
        //THREE.DRACOLoader.setDecoderPath('/examples/js/libs/draco');
        //loader.setDRACOLoader(new THREE.DRACOLoader());

        //// Load a glTF resource
        //loader.load(
        //    // resource URL
        //    'models/myGarage.glb',
        //    // called when the resource is loaded
        //    function (gltf) {

        //        scene.add(gltf.scene);

        //        gltf.animations; // Array<THREE.AnimationClip>
        //        gltf.scene; // THREE.Scene
        //        gltf.scenes; // Array<THREE.Scene>
        //        gltf.cameras; // Array<THREE.Camera>
        //        gltf.asset; // Object

        //    },
        //    // called while loading is progressing
        //    function (xhr) {

        //        console.log((xhr.loaded / xhr.total * 100) + '% loaded');

        //    },
        //    // called when loading has errors
        //    function (error) {

        //        console.log('An error happened');

        //    }
        //);


        var OBJLoader = new THREE.OBJLoader();
        var mtlLoader = new THREE.MTLLoader();
        OBJLoader.setPath('models/');
        mtlLoader.setPath('models/');

        mtlLoader.load('myGarage.mtl', function (materials) {
            materials.preload();
            materials.wireframe = false;
            OBJLoader.setMaterials(materials);
            // load a resource
            OBJLoader.load(
                // resource URL
                'myGarage.obj',

                // called when resource is loaded
                function (garage) {
                    var scalar = 35;
                    garage.scale.set(2 / scalar, 1 / scalar, 1 / scalar);

                    var group = new THREE.Group();
                    group.add(garage);

                    scene.add(group);

                    garage.position.x = 15;
                    garage.position.y = 0.001;
                    garage.position.z = 1.429;

                },
                // called when loading has errors
                function (error) {
                    console.log('An error happened - garage');
                }
            );
        });
    }
    function placeRadioTower(scene) {
        var OBJLoader = new THREE.OBJLoader();
        var mtlLoader = new THREE.MTLLoader();
        OBJLoader.setPath('models/');
        mtlLoader.setPath('models/');

        //Air Traffic Control Tower
        mtlLoader.load('PUSHILIN_radio_tower.mtl', function (materials) {
            //mtlLoader.load('atct.mtl', function (materials) {

            materials.preload();
            materials.wireframe = false;
            OBJLoader.setMaterials(materials);
            // load a resource
            OBJLoader.load(
                // resource URL
                'PUSHILIN_radio_tower.obj',
                //'atct.obj',

                // called when resource is loaded
                function (tower) {
                    var scalar = 7;
                    tower.scale.set(scalar - 1, scalar - 3, scalar - 1);

                    var group = new THREE.Group();
                    group.add(tower);

                    scene.add(group);
                    //worldObjects[command.parameters.guid] = group;

                    tower.position.x = 2;
                    tower.position.y = 9;
                    tower.position.z = 3;

                },
                // called when loading has errors
                function (error) {
                    console.log('An error happened - radiotower');
                }
            );
        });
    }